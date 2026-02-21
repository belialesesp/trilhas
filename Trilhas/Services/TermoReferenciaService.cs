using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Trilhas.Data;
using Trilhas.Data.Model.Exceptions;
using Trilhas.Data.Model.TermosReferencia;

namespace Trilhas.Services
{
    public class TermoReferenciaService
    {
        private readonly ApplicationDbContext _context;
        private readonly MinioService _minioService;
        private readonly NotificationService _notificationService;

        public TermoReferenciaService(
            ApplicationDbContext context, 
            MinioService minioService,
            NotificationService notificationService)
        {
            _context = context;
            _minioService = minioService;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Processes an uploaded PDF document and extracts Termo de Referência data
        /// </summary>
        /// <summary>
        /// Processes an uploaded PDF document and extracts Termo de Referência data
        /// UPDATED: Now creates individual contractor slots
        /// </summary>
        public TermoDeReferencia ProcessarDocumentoPDF(string userId, Stream documentStream, string fileName, int ano)
        {
            var termo = new TermoDeReferencia
            {
                Titulo = ExtrairTituloDocumento(documentStream),
                Ano = ano,
                Status = "Rascunho",
                CreatorUserId = userId,
                CreationTime = DateTime.Now
            };

            try
            {
                documentStream.Position = 0;
                var dadosExtraidos = ExtrairDadosDoPDF(documentStream);
                
                termo.NumeroDocumento = dadosExtraidos.NumeroDocumento;
                termo.Descricao = dadosExtraidos.Descricao;
                termo.Demandante = dadosExtraidos.Demandante;
                termo.DataInicio = dadosExtraidos.DataInicio;
                termo.DataTermino = dadosExtraidos.DataTermino;
                termo.Duracao = dadosExtraidos.Duracao;
                termo.Itens = dadosExtraidos.Itens;

                if (termo.Itens == null || !termo.Itens.Any())
                {
                    throw new TrilhasException("Não foi possível extrair os profissionais necessários do documento.");
                }

                // CREATE INDIVIDUAL CONTRACTOR SLOTS FOR EACH ITEM
                foreach (var item in termo.Itens)
                {
                    // Create one slot for each quantity needed
                    for (int i = 1; i <= item.Quantidade; i++)
                    {
                        item.Slots.Add(new ContratadoSlot
                        {
                            NumeroSlot = i,
                            NomeContratado = null,
                            Ateste = false,
                            CreationTime = DateTime.Now
                        });
                    }
                }

                documentStream.Position = 0;
                MemoryStream memoryStream;
                if (documentStream is MemoryStream ms)
                {
                    memoryStream = ms;
                }
                else
                {
                    memoryStream = new MemoryStream();
                    documentStream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                }
                
                var arquivo = new Arquivo 
                { 
                    Nome = $"termos-referencia/{ano}/{fileName}",
                    ArquivoStream = memoryStream
                };
                _minioService.SalvarImagemEixo(arquivo);
                termo.CaminhoArquivoOriginal = arquivo.Nome;

                _context.TermosDeReferencia.Add(termo);
                _context.SaveChanges();

                return termo;
            }
            catch (TrilhasException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new TrilhasException($"Erro ao processar documento: {ex.Message}");
            }
            // CREATE INDIVIDUAL CONTRACTOR SLOTS FOR EACH ITEM
foreach (var item in termo.Itens)
{
    // Create one slot for each quantity needed
    for (int i = 1; i <= item.Quantidade; i++)
    {
        item.Slots.Add(new ContratadoSlot
        {
            NumeroSlot = i,
            NomeContratado = null,
            Ateste = false,
            CreationTime = DateTime.Now
        });
    }
}
        }

        private string ExtrairTituloDocumento(Stream pdfStream)
        {
            try
            {
                using (var pdfReader = new PdfReader(pdfStream))
                using (var pdfDocument = new PdfDocument(pdfReader))
                {
                    var page = pdfDocument.GetFirstPage();
                    var strategy = new SimpleTextExtractionStrategy();
                    string text = PdfTextExtractor.GetTextFromPage(page, strategy);

                    var match = Regex.Match(text, @"2\.1\.\s*Título do Projeto\s*[:\-]?\s*([^\n]+)", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        return match.Groups[1].Value.Trim();
                    }
                    return "Termo de Referência";
                }
            }
            catch
            {
                return "Termo de Referência";
            }
        }

        private DadosExtraidosPDF ExtrairDadosDoPDF(Stream pdfStream)
        {
            var dados = new DadosExtraidosPDF
            {
                Itens = new List<TermoReferenciaItem>()
            };

            using (var pdfReader = new PdfReader(pdfStream))
            using (var pdfDocument = new PdfDocument(pdfReader))
            {
                string fullText = "";
                
                for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                {
                    var page = pdfDocument.GetPage(i);
                    var strategy = new SimpleTextExtractionStrategy();
                    string pageText = PdfTextExtractor.GetTextFromPage(page, strategy);
                    fullText += pageText + "\n";
                }

                try
                {
                    string logPath = Path.Combine(Path.GetTempPath(), "pdf_extracted_text.txt");
                    File.WriteAllText(logPath, fullText);
                }
                catch { }

                var demandanteMatch = Regex.Match(fullText, @"Nome:\s*([^\n]+)", RegexOptions.IgnoreCase);
                if (demandanteMatch.Success)
                {
                    dados.Demandante = demandanteMatch.Groups[1].Value.Trim();
                }

                var inicioMatch = Regex.Match(fullText, @"Início:\s*([^\n]+)", RegexOptions.IgnoreCase);
                if (inicioMatch.Success)
                {
                    dados.DataInicio = inicioMatch.Groups[1].Value.Trim();
                }

                var terminoMatch = Regex.Match(fullText, @"Término:\s*([^\n]+)", RegexOptions.IgnoreCase);
                if (terminoMatch.Success)
                {
                    dados.DataTermino = terminoMatch.Groups[1].Value.Trim();
                }

                var duracaoMatch = Regex.Match(fullText, @"Duração Total:\s*(\d+)", RegexOptions.IgnoreCase);
                if (duracaoMatch.Success)
                {
                    dados.Duracao = int.Parse(duracaoMatch.Groups[1].Value);
                }

                try
                {
                    dados.Itens = ExtrairItensDaTabela(fullText);
                }
                catch (TrilhasException ex)
                {
                    string logPath = Path.Combine(Path.GetTempPath(), "pdf_extracted_text.txt");
                    throw new TrilhasException($"{ex.Message}\n\nTexto extraído salvo em: {logPath}");
                }
            }

            return dados;
        }

        /// <summary>
        /// Extract professional requirements from Anexo II table
        /// BALANCED: Gets full course names without over-carrying forward
        /// </summary>
        private List<TermoReferenciaItem> ExtrairItensDaTabela(string texto)
        {
            var itens = new List<TermoReferenciaItem>();

            // Find the ACTUAL Anexo II table
            int startIdx = texto.IndexOf("Anexo II – PLANILHA", StringComparison.OrdinalIgnoreCase);
            if (startIdx == -1)
            {
                startIdx = texto.IndexOf("Anexo II - PLANILHA", StringComparison.OrdinalIgnoreCase);
            }
            if (startIdx == -1)
            {
                startIdx = texto.LastIndexOf("Anexo II", StringComparison.OrdinalIgnoreCase);
            }
            if (startIdx == -1)
            {
                throw new TrilhasException("Seção 'Anexo II' não encontrada no PDF.");
            }

            int endIdx = texto.IndexOf("Anexo III", startIdx, StringComparison.OrdinalIgnoreCase);
            if (endIdx == -1) endIdx = texto.IndexOf("Observação:", startIdx, StringComparison.OrdinalIgnoreCase);
            if (endIdx == -1) endIdx = texto.Length;

            string anexoText = texto.Substring(startIdx, endIdx - startIdx);
            var lines = anexoText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            string currentCourse = "";
            int itemsSinceLastCourse = 0; // Track how many items since we found a course name
            
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                
                // Skip headers
                if (string.IsNullOrWhiteSpace(line) || 
                    line.Contains("Curso") ||
                    line.Contains("Carga") ||
                    line.Contains("Horária") ||
                    line.Contains("Modalidade") ||
                    line.Contains("Alunos") ||
                    line.Contains("Valor") ||
                    line.Contains("Total") ||
                    line.Contains("PÁGINA") ||
                    line.Contains("Quant") ||
                    line.Contains("Turm") ||
                    line.Contains("Encarg"))
                {
                    continue;
                }

                // Look for professional category
                bool hasDocente = line.Contains("DOCENTE");
                bool hasModerador = line.Contains("MODERADOR");
                bool hasConteudista = line.Contains("CONTEUDISTA");
                
                // Check next line for CONTEUDISTA if current line has DOCENTE
                int skipLines = 0;
                if (hasDocente && !hasConteudista && i + 1 < lines.Length)
                {
                    string nextLine = lines[i + 1].Trim();
                    if (nextLine.Contains("CONTEUDISTA") || nextLine == "CONTEUDISTA")
                    {
                        hasConteudista = true;
                        skipLines = 1; // We'll skip this line when collecting data
                    }
                }

                if (hasDocente || hasModerador)
                {
                    // Determine category
                    string categoria = "";
                    if (hasDocente && hasConteudista)
                    {
                        categoria = "DOCENTE_CONTEUDISTA";
                    }
                    else if (hasModerador)
                    {
                        categoria = "MODERADOR";
                    }
                    else if (hasDocente)
                    {
                        categoria = "DOCENTE";
                    }

                    // Search backward for course name
                    List<string> courseLines = new List<string>();
                    bool foundValidCourse = false;
                    
                    for (int j = i - 1; j >= Math.Max(0, i - 25); j--)
                    {
                        string prev = lines[j].Trim();
                        
                        // Skip truly empty lines but don't count them as a stop
                        if (string.IsNullOrWhiteSpace(prev))
                        {
                            continue;
                        }
                        
                        // HARD STOPS - these are definite boundaries
                        if (prev.Contains("Governo do Estado") ||
                            prev.Contains("Corpo de Bombeiro") ||
                            prev.Contains("Coordenadoria Estadual") ||
                            prev.Contains("defesacivil@bombeiros") ||
                            prev.Contains("PÁGINA") ||
                            prev.Contains("E-DOCS") ||
                            prev.Contains("DOCUMENTO ORIGINAL") ||
                            prev.Contains("Rua:") ||
                            prev.Contains("CEP:") ||
                            prev.Contains("Curso Profissional") ||
                            prev.Contains("Total (R$)") ||
                            prev.StartsWith("27 ") ||
                            Regex.IsMatch(prev, @"^\d{4,}$"))
                        {
                            break;
                        }

                        // DATA lines - these mean we've passed the course name area
                        // BUT: Only stop if we already have course lines
                        bool isDataLine = prev.Contains("R$") ||
                                        prev.Contains("PRESENCIAL") ||
                                        prev.Contains("EAD") ||
                                        prev.Contains("SÍNCRONO") ||
                                        prev.Contains("HÍBRIDO") ||
                                        Regex.IsMatch(prev, @"^\d+$") ||
                                        Regex.IsMatch(prev, @"^\d{2}/\d{2}/\d{4}$") ||
                                        Regex.IsMatch(prev, @"^\d+\s+\d+$");
                        
                        if (isDataLine && courseLines.Count > 0)
                        {
                            // We have course lines and hit data - stop here
                            break;
                        }
                        else if (isDataLine)
                        {
                            // Data line but no course yet - skip it
                            continue;
                        }

                        // Month names alone are not course names
                        bool isMonth = Regex.IsMatch(prev, @"^(JANEIRO|FEVEREIRO|MAR[ÇC]O|ABRIL|MAIO|JUNHO|JULHO|AGOSTO|SETEMBRO|OUTUBRO|NOVEMBRO|DEZEMBRO)$", RegexOptions.IgnoreCase);
                        
                        // VALID course name line
                        if (prev == prev.ToUpper() && 
                            prev.Length > 2 && 
                            !isMonth &&
                            !prev.Contains("DOCENTE") && 
                            !prev.Contains("MODERADOR") &&
                            !prev.Contains(":"))  // Keep colon check for data labels
                        {
                            courseLines.Insert(0, prev);
                            foundValidCourse = true;
                        }
                    }

                    // Update current course if we found a valid new one
                    if (foundValidCourse && courseLines.Count > 0)
                    {
                        string newCourse = string.Join(" ", courseLines);
                        
                        // Validate it's not junk
                        if (newCourse.Length >= 5 && 
                            !newCourse.Contains("E-DOCS") &&
                            !newCourse.Contains("PÁGINA"))
                        {
                            currentCourse = newCourse;
                            itemsSinceLastCourse = 0;
                        }
                    }
                    else
                    {
                        // No new course found
                        itemsSinceLastCourse++;
                        
                        // If we've gone too long without finding a course, reset
                        // This prevents infinite carry-forward
                        if (itemsSinceLastCourse > 5)
                        {
                            currentCourse = "";
                        }
                    }

                    // Skip if no valid course
                    if (string.IsNullOrWhiteSpace(currentCourse))
                    {
                        continue;
                    }

                    // Get data from this line and next lines
                    string dataText = line;
                    
                    // Start collecting from the appropriate line (skip CONTEUDISTA if it was on separate line)
                    int startJ = i + 1 + skipLines;
                    
                    for (int j = startJ; j < Math.Min(lines.Length, startJ + 5); j++)
                    {
                        string next = lines[j].Trim();
                        if (next.Contains("DOCENTE") || next.Contains("MODERADOR") || next == "CONTEUDISTA")
                            break;
                        dataText += " " + next;
                    }

                    // Remove category keywords
                    dataText = dataText.Replace("DOCENTE CONTEUDISTA", " ")
                                      .Replace("DOCENTE", " ")
                                      .Replace("MODERADOR", " ")
                                      .Trim();

                    // Extract numbers
                    var nums = Regex.Matches(dataText, @"\d+");
                    if (nums.Count < 2) continue;

                    int qtd = int.Parse(nums[0].Value);
                    decimal ch = decimal.Parse(nums[1].Value);

                    // Extract month
                    var mesMatch = Regex.Match(dataText, @"(JANEIRO|FEVEREIRO|MAR[ÇC]O|ABRIL|MAIO|JUNHO|JULHO|AGOSTO|SETEMBRO|OUTUBRO|NOVEMBRO|DEZEMBRO)", RegexOptions.IgnoreCase);
                    string mes = mesMatch.Success ? mesMatch.Value.ToUpper() : "";

                    // Extract date
                    var dateMatch = Regex.Match(dataText, @"(\d{2}/\d{2}/\d{4})");
                    DateTime? data = null;
                    if (dateMatch.Success)
                    {
                        DateTime.TryParseExact(dateMatch.Value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d);
                        data = d;
                    }

                    itens.Add(new TermoReferenciaItem
                    {
                        Curso = currentCourse,
                        Profissional = categoria,
                        Quantidade = qtd,
                        CargaHoraria = ch,
                        MesExecucao = mes,
                        DataOferta = data,
                        Contratados = 0,
                        CreatorUserId = "",
                        CreationTime = DateTime.Now
                    });
                    
                    // Skip the CONTEUDISTA line if it was on a separate line
                    if (skipLines > 0)
                    {
                        i += skipLines;
                    }
                }
            }

            if (itens.Count == 0)
            {
                throw new TrilhasException("Nenhum item extraído. Verifique se o PDF tem a tabela Anexo II no formato esperado.");
            }

            return itens;
        }

        public List<AlertaContratacao> VerificarCursosProximos()
        {
            var alertas = new List<AlertaContratacao>();
            var dataAtual = DateTime.Now;
            var dataLimite = dataAtual.AddDays(15);

            var termosAtivos = _context.TermosDeReferencia
                .Include(t => t.Itens)
                .Where(t => t.DeletionTime == null && 
                           (t.Status == "Aprovado" || t.Status == "Em Execução"))
                .ToList();

            foreach (var termo in termosAtivos)
            {
                foreach (var item in termo.Itens.Where(i => i.DeletionTime == null))
                {
                    if (item.Contratados < item.Quantidade && item.DataOferta.HasValue)
                    {
                        if (item.DataOferta.Value > dataAtual && item.DataOferta.Value <= dataLimite)
                        {
                            var vagas = item.Quantidade - item.Contratados;
                            
                            alertas.Add(new AlertaContratacao
                            {
                                TermoId = termo.Id,
                                TermoTitulo = termo.Titulo,
                                Demandante = termo.Demandante,
                                Curso = item.Curso,
                                Categoria = item.Profissional,
                                VagasRestantes = vagas,
                                DataOferta = item.DataOferta.Value,
                                DiasRestantes = (item.DataOferta.Value - dataAtual).Days
                            });
                        }
                    }
                }
            }

            return alertas.OrderBy(a => a.DataOferta).ToList();
        }

        public void EnviarNotificacoesContratacao()
{
    var alertas = VerificarCursosProximos();
    
    if (!alertas.Any())
    {
        return;
    }

    // Query UserProfiles table instead of Identity tables
    var gedthUsers = _context.UserProfiles
        .Where(u => u.Role == "GEDTH" && 
                   u.ReceiveNotifications && 
                   !string.IsNullOrEmpty(u.Email))
        .ToList();

    Console.WriteLine($"📧 Sending notifications to {gedthUsers.Count} GEDTH users");

    foreach (var user in gedthUsers)
    {
        try
        {
            _notificationService.EnviarEmailAlertaContratacao(user.Email, user.Name, alertas);
            _notificationService.CriarNotificacaoInterna(user.UserId, alertas);
            
            Console.WriteLine($"✅ Notification sent to: {user.Email}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to notify {user.Email}: {ex.Message}");
        }
    }
}

        public void AtualizarContratados(long itemId, int novoValor)
        {
            var item = _context.TermoReferenciaItens.Find(itemId);
            if (item == null)
            {
                throw new TrilhasException("Item não encontrado.");
            }

            if (novoValor < 0 || novoValor > item.Quantidade)
            {
                throw new TrilhasException($"Valor inválido. Deve estar entre 0 e {item.Quantidade}.");
            }

            item.Contratados = novoValor;
            item.LastModificationTime = DateTime.Now;
            _context.SaveChanges();
        }

        public TermoDeReferencia RecuperarTermo(long id, bool incluirExcluidos)
{
    var query = _context.TermosDeReferencia
        .Include(t => t.Itens)
            .ThenInclude(i => i.Slots)  // <-- ADD THIS
        .Where(t => t.Id == id);

    if (!incluirExcluidos)
    {
        query = query.Where(t => t.DeletionTime == null);
    }

    return query.FirstOrDefault();
}

        public TermoReferenciaItem RecuperarItem(long id)
        {
            return _context.TermoReferenciaItens.Find(id);
        }

        public void SalvarAlteracoes()
        {
            _context.SaveChanges();
        }

        public List<TermoDeReferencia> BuscarTermos(int? ano, string status, bool incluirExcluidos, int start, int count)
        {
            var query = _context.TermosDeReferencia
                .Include(t => t.Itens)
                .AsQueryable();

            if (ano.HasValue)
            {
                query = query.Where(t => t.Ano == ano.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.Status == status);
            }

            if (!incluirExcluidos)
            {
                query = query.Where(t => t.DeletionTime == null);
            }

            return query
                .OrderByDescending(t => t.CreationTime)
                .Skip(start)
                .Take(count)
                .ToList();
        }

        public void ExcluirTermo(long id, string userId)
        {
            var termo = _context.TermosDeReferencia.Find(id);
            if (termo == null)
            {
                throw new TrilhasException("Termo não encontrado.");
            }

            termo.DeletionTime = DateTime.Now;
            termo.DeletionUserId = userId;
            _context.SaveChanges();
        }
    }

    public class DadosExtraidosPDF
    {
        public string NumeroDocumento { get; set; }
        public string Descricao { get; set; }
        public string Demandante { get; set; }
        public string DataInicio { get; set; }
        public string DataTermino { get; set; }
        public int Duracao { get; set; }
        public List<TermoReferenciaItem> Itens { get; set; }
    }

    public class AlertaContratacao
    {
        public long TermoId { get; set; }
        public string TermoTitulo { get; set; }
        public string Demandante { get; set; }
        public string Curso { get; set; }
        public string Categoria { get; set; }
        public int VagasRestantes { get; set; }
        public DateTime DataOferta { get; set; }
        public int DiasRestantes { get; set; }
    }
}