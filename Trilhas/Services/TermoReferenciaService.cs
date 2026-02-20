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
        /// FOCUSED ONLY ON ANEXO II TABLE
        /// </summary>
        private List<TermoReferenciaItem> ExtrairItensDaTabela(string texto)
        {
            var itens = new List<TermoReferenciaItem>();

            // Find the ACTUAL Anexo II table (not just references to it)
            int startIdx = texto.IndexOf("Anexo II – PLANILHA", StringComparison.OrdinalIgnoreCase);
            if (startIdx == -1)
            {
                startIdx = texto.IndexOf("Anexo II - PLANILHA", StringComparison.OrdinalIgnoreCase);
            }
            if (startIdx == -1)
            {
                // Fallback: find last occurrence of "Anexo II"
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
            
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                
                if (string.IsNullOrWhiteSpace(line) || 
                    line.Contains("Curso Profissional") ||
                    line.Contains("Carga Horária") ||
                    line.Contains("Modalidade") ||
                    line.Contains("Alunos p/") ||
                    line.Contains("Valor Hora") ||
                    line.Contains("Total (R$)") ||
                    line.Contains("PÁGINA"))
                {
                    continue;
                }

                if (line.Contains("DOCENTE") || line.Contains("MODERADOR"))
                {
                    // Determine category
                    string categoria = "";
                    if (line.Contains("DOCENTE") && line.Contains("CONTEUDISTA"))
                    {
                        categoria = "DOCENTE_CONTEUDISTA";
                    }
                    else if (line.Contains("MODERADOR"))
                    {
                        categoria = "MODERADOR";
                    }
                    else if (line.Contains("DOCENTE"))
                    {
                        categoria = "DOCENTE";
                    }

                    // Find course name from previous lines - FIXED LOGIC
                    List<string> courseParts = new List<string>();
                    bool foundEmptyLine = false;
                    
                    for (int j = i - 1; j >= Math.Max(0, i - 15); j--)
                    {
                        string prevLine = lines[j].Trim();
                        
                        // STOP CONDITIONS - these indicate we've gone too far back
                        if (prevLine.Contains("Curso Profissional") ||
                            prevLine.Contains("Total (R$)") ||
                            prevLine.Contains("PÁGINA") ||
                            prevLine.Contains("Governo do Estado") ||
                            prevLine.Contains("Corpo de Bombeiro") ||
                            prevLine.Contains("defesacivil@bombeiros"))
                        {
                            break;
                        }

                        // If we hit an empty line, mark it but continue one more iteration
                        if (string.IsNullOrWhiteSpace(prevLine))
                        {
                            if (foundEmptyLine)
                            {
                                // Second empty line - definitely stop
                                break;
                            }
                            foundEmptyLine = true;
                            continue;
                        }

                        // EXCLUDE data lines that shouldn't be in course name
                        if (prevLine.Contains("R$") ||  // ANY line with R$ is data
                            prevLine.Contains(":") ||   // Lines with colons are usually data
                            prevLine.Contains(",") ||   // Lines with commas are usually numbers/data
                            prevLine.StartsWith("R$") ||
                            prevLine.Contains("PRESENCIAL") ||
                            prevLine.Contains("EAD") ||
                            prevLine.Contains("SÍNCRONO") ||
                            prevLine.Contains("HÍBRIDO") ||
                            Regex.IsMatch(prevLine, @"^\d+$") ||  // Just a number
                            Regex.IsMatch(prevLine, @"^\d{2}/\d{2}/\d{4}$") ||  // Just a date
                            Regex.IsMatch(prevLine, @"R\$") ||  // Contains R$
                            Regex.IsMatch(prevLine, @"^\d+\s+\d+$"))  // Two numbers (like "1 40")
                        {
                            // This is data, not course name - stop here
                            break;
                        }

                        // If it's uppercase and substantial, it's likely part of course name
                        if (prevLine == prevLine.ToUpper() && prevLine.Length > 2)
                        {
                            courseParts.Insert(0, prevLine);
                        }
                        else if (courseParts.Count > 0)
                        {
                            // We found some course parts, then hit a non-uppercase line - stop
                            break;
                        }
                    }

                    if (courseParts.Count > 0)
                    {
                        currentCourse = string.Join(" ", courseParts);
                    }
                    // If no course name found, keep the previous one (carry forward)


                    string dataLine = line.Replace("DOCENTE CONTEUDISTA", "").Replace("DOCENTE", "").Replace("MODERADOR", "").Trim();
                    
                    for (int j = i + 1; j < Math.Min(lines.Length, i + 5); j++)
                    {
                        string nextLine = lines[j].Trim();
                        if (nextLine.Contains("DOCENTE") || nextLine.Contains("MODERADOR"))
                            break;
                        dataLine += " " + nextLine;
                    }

                    var numbers = Regex.Matches(dataLine, @"\d+");
                    if (numbers.Count < 2) continue;

                    int quantidade = int.Parse(numbers[0].Value);
                    decimal cargaHoraria = decimal.Parse(numbers[1].Value);

                    var monthMatch = Regex.Match(dataLine, @"(JANEIRO|FEVEREIRO|MAR[ÇC]O|ABRIL|MAIO|JUNHO|JULHO|AGOSTO|SETEMBRO|OUTUBRO|NOVEMBRO|DEZEMBRO)", RegexOptions.IgnoreCase);
                    string mes = monthMatch.Success ? monthMatch.Value.ToUpper() : "";

                    var dateMatch = Regex.Match(dataLine, @"(\d{2}/\d{2}/\d{4})");
                    DateTime? dataOferta = null;
                    if (dateMatch.Success)
                    {
                        DateTime.TryParseExact(dateMatch.Value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed);
                        dataOferta = parsed;
                    }

                    itens.Add(new TermoReferenciaItem
                    {
                        Curso = currentCourse,
                        Profissional = categoria,
                        Quantidade = quantidade,
                        CargaHoraria = cargaHoraria,
                        MesExecucao = mes,
                        DataOferta = dataOferta,
                        Contratados = 0,
                        CreatorUserId = "",
                        CreationTime = DateTime.Now
                    });
                }
            }

            if (itens.Count == 0)
            {
                throw new TrilhasException("Nenhum item extraído da tabela Anexo II.");
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