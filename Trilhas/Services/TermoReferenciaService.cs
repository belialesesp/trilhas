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
                    // Read first 3 pages to find title
                    string text = "";
                    int pagesToRead = Math.Min(3, pdfDocument.GetNumberOfPages());
                    for (int i = 1; i <= pagesToRead; i++)
                    {
                        var page = pdfDocument.GetPage(i);
                        var strategy = new SimpleTextExtractionStrategy();
                        text += PdfTextExtractor.GetTextFromPage(page, strategy) + "\n";
                    }

                    // Try multiple common title patterns (most specific first)
                    string[] titlePatterns = new[]
                    {
                        @"T[ií]tulo\s+do\s+Projeto\s*[:\-–]?\s*([^\n]+)",
                        @"OBJETO\s*[:\-–]\s*([^\n]+)",
                        @"Objeto\s*[:\-–]\s*([^\n]+)",
                        @"TERMO\s+DE\s+REFER[ÊE]NCIA\s*[:\-–]\s*([^\n]+)",
                    };

                    foreach (var pattern in titlePatterns)
                    {
                        var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            var title = match.Groups[1].Value.Trim();
                            // Sanity check: title should be reasonable length
                            if (title.Length >= 5 && title.Length <= 300)
                                return title;
                        }
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

                // Log extracted text for debugging
                try
                {
                    string logPath = Path.Combine(Path.GetTempPath(), "pdf_extracted_text.txt");
                    File.WriteAllText(logPath, fullText);
                }
                catch { }

                // --- DEMANDANTE ---
                // Try multiple patterns for the requesting entity
                string[] demandantePatterns = new[]
                {
                    @"[Óó]rg[aã]o\s*(?:Demandante|Requisitante)\s*[:\-–]\s*([^\n]+)",
                    @"Demandante\s*[:\-–]\s*([^\n]+)",
                    @"Entidade\s*[:\-–]\s*([^\n]+)",
                    @"Nome\s*[:\-–]\s*([^\n]+)",
                    @"CONTRATANTE\s*[:\-–]\s*([^\n]+)",
                };

                foreach (var pattern in demandantePatterns)
                {
                    var match = Regex.Match(fullText, pattern, RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        var value = match.Groups[1].Value.Trim();
                        // Filter out values that are clearly not an entity name
                        if (value.Length >= 3 && !Regex.IsMatch(value, @"^\d+$"))
                        {
                            dados.Demandante = value;
                            break;
                        }
                    }
                }

                // --- DATES ---
                string[] inicioPatterns = new[]
                {
                    @"In[ií]cio\s*[:\-–]\s*([^\n]+)",
                    @"Data\s+de\s+In[ií]cio\s*[:\-–]\s*([^\n]+)",
                    @"Per[ií]odo\s*[:\-–]\s*([^\n]+?)(?:\s+a\s+|\s*[-–]\s*)",
                };

                foreach (var pattern in inicioPatterns)
                {
                    var match = Regex.Match(fullText, pattern, RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        dados.DataInicio = match.Groups[1].Value.Trim();
                        break;
                    }
                }

                string[] terminoPatterns = new[]
                {
                    @"T[ée]rmino\s*[:\-–]\s*([^\n]+)",
                    @"Data\s+de\s+T[ée]rmino\s*[:\-–]\s*([^\n]+)",
                    @"(?:a|até|[-–])\s*((?:JANEIRO|FEVEREIRO|MAR[ÇC]O|ABRIL|MAIO|JUNHO|JULHO|AGOSTO|SETEMBRO|OUTUBRO|NOVEMBRO|DEZEMBRO)\s*/?\s*\d{4})",
                };

                foreach (var pattern in terminoPatterns)
                {
                    var match = Regex.Match(fullText, pattern, RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        dados.DataTermino = match.Groups[1].Value.Trim();
                        break;
                    }
                }

                // --- DURATION ---
                var duracaoPatterns = new[]
                {
                    @"Dura[çc][aã]o\s*(?:Total)?\s*[:\-–]\s*(\d+)",
                    @"(\d+)\s*meses",
                };

                foreach (var pattern in duracaoPatterns)
                {
                    var match = Regex.Match(fullText, pattern, RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        dados.Duracao = int.Parse(match.Groups[1].Value);
                        break;
                    }
                }

                // --- ITEMS FROM ANEXO II ---
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

            // ---- FIND ANEXO II SECTION ----
            int startIdx = -1;
            string[] anexoMarkers = new[]
            {
                "Anexo II – PLANILHA",
                "Anexo II - PLANILHA",
                "ANEXO II – PLANILHA",
                "ANEXO II - PLANILHA",
                "Anexo II",
                "ANEXO II"
            };

            foreach (var marker in anexoMarkers)
            {
                startIdx = texto.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                if (startIdx != -1) break;
            }

            if (startIdx == -1)
            {
                throw new TrilhasException("Seção 'Anexo II' não encontrada no PDF. Certifique-se de que o documento contém a planilha de profissionais.");
            }

            // ---- FIND END OF SECTION ----
            // Look for Anexo III or other common section endings
            int endIdx = texto.Length;
            string[] endMarkers = new[]
            {
                "Anexo III", "ANEXO III",
                "Anexo IV", "ANEXO IV",
                "Observação:", "OBSERVAÇÃO:",
                "Observações:", "OBSERVAÇÕES:",
                "VALOR GLOBAL", "Valor Global"
            };

            foreach (var marker in endMarkers)
            {
                int idx = texto.IndexOf(marker, startIdx + 10, StringComparison.OrdinalIgnoreCase);
                if (idx != -1 && idx < endIdx)
                {
                    endIdx = idx;
                }
            }

            string anexoText = texto.Substring(startIdx, endIdx - startIdx);
            var lines = anexoText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            string currentCourse = "";
            int itemsSinceLastCourse = 0;

            // ---- GENERIC HEADER/NOISE DETECTION ----
            // Instead of hardcoding specific headers, detect them by pattern
            Regex headerPattern = new Regex(
                @"^(Curso|Carga|Hor[áa]ria|Modalidade|Alunos|Valor|Total|Quant|Turm|Encarg|Profissional|Tipo|Mês|Data|Nr\.|Item)",
                RegexOptions.IgnoreCase);

            // Generic noise: lines that are clearly not course names or professional data
            // Detects page headers, footers, addresses, emails, URLs, document IDs
            Regex noisePattern = new Regex(
                @"(^PÁGINA\s|" +
                @"E-DOCS|" +
                @"DOCUMENTO ORIGINAL|" +
                @"@\w+\.\w+|" +                     // emails
                @"^https?://|" +                      // URLs
                @"^www\.|" +                          // URLs
                @"CEP\s*[:\-]|" +                     // postal code
                @"^Rua\s|^Av\.\s|^Avenida\s|" +      // addresses
                @"^Tel[\.:]|^Fone[\.:]|" +            // phone
                @"CNPJ|CPF|" +                        // IDs
                @"^\d{2}\.\d{3}\.\d{3}|" +            // CNPJ format
                @"Governo\s+d[oe]|" +                 // government headers
                @"^Secretaria\s+d[eao]|" +            // government department
                @"Fls\.\s*\d|" +                      // page numbering
                @"^Processo\s*n|" +                    // process number
                @"^\d{4}\.\d+|" +                     // document numbers like 2024.12345
                @"^_{3,}|^-{3,}|^\*{3,})",            // divider lines
                RegexOptions.IgnoreCase);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                // Skip empty, header, or noise lines
                if (string.IsNullOrWhiteSpace(line) ||
                    headerPattern.IsMatch(line) ||
                    noisePattern.IsMatch(line))
                {
                    continue;
                }

                // ---- LOOK FOR PROFESSIONAL CATEGORY ----
                bool hasDocente = line.Contains("DOCENTE");
                bool hasModerador = line.Contains("MODERADOR");
                bool hasConteudista = line.Contains("CONTEUDISTA");

                // Check next line for CONTEUDISTA (sometimes on separate line)
                int skipLines = 0;
                if (hasDocente && !hasConteudista && i + 1 < lines.Length)
                {
                    string nextLine = lines[i + 1].Trim();
                    if (nextLine.Contains("CONTEUDISTA") || nextLine == "CONTEUDISTA")
                    {
                        hasConteudista = true;
                        skipLines = 1;
                    }
                }

                if (hasDocente || hasModerador)
                {
                    // Determine category
                    string categoria = "";
                    if (hasDocente && hasConteudista)
                        categoria = "DOCENTE_CONTEUDISTA";
                    else if (hasModerador)
                        categoria = "MODERADOR";
                    else if (hasDocente)
                        categoria = "DOCENTE";

                    // ---- SEARCH BACKWARD FOR COURSE NAME ----
                    List<string> courseLines = new List<string>();
                    bool foundValidCourse = false;

                    for (int j = i - 1; j >= Math.Max(0, i - 25); j--)
                    {
                        string prev = lines[j].Trim();

                        if (string.IsNullOrWhiteSpace(prev))
                            continue;

                        // Stop at noise/headers
                        if (noisePattern.IsMatch(prev) || headerPattern.IsMatch(prev))
                            break;

                        // Stop at another professional category line (means we've gone too far back)
                        if (prev.Contains("DOCENTE") || prev.Contains("MODERADOR"))
                            break;

                        // Data lines = stop if we already have course lines
                        bool isDataLine = prev.Contains("R$") ||
                                          Regex.IsMatch(prev, @"^(PRESENCIAL|EAD|EAD SÍNCRONO|SÍNCRONO|HÍBRIDO|SEMIPRESENCIAL)$", RegexOptions.IgnoreCase) ||
                                          Regex.IsMatch(prev, @"^\d+$") ||
                                          Regex.IsMatch(prev, @"^\d{2}/\d{2}/\d{4}$") ||
                                          Regex.IsMatch(prev, @"^\d+\s+\d+$");

                        if (isDataLine && courseLines.Count > 0)
                            break;
                        else if (isDataLine)
                            continue;

                        // Month names alone are not course names
                        bool isMonth = Regex.IsMatch(prev,
                            @"^(JANEIRO|FEVEREIRO|MAR[ÇC]O|ABRIL|MAIO|JUNHO|JULHO|AGOSTO|SETEMBRO|OUTUBRO|NOVEMBRO|DEZEMBRO)$",
                            RegexOptions.IgnoreCase);

                        // Valid course name: uppercase, reasonable length, not a keyword
                        if (prev == prev.ToUpper() &&
                            prev.Length > 2 &&
                            !isMonth &&
                            !prev.Contains("DOCENTE") &&
                            !prev.Contains("MODERADOR") &&
                            !prev.Contains(":"))
                        {
                            courseLines.Insert(0, prev);
                            foundValidCourse = true;
                        }
                    }

                    // Update current course if we found a valid new one
                    if (foundValidCourse && courseLines.Count > 0)
                    {
                        string newCourse = string.Join(" ", courseLines);

                        if (newCourse.Length >= 5 &&
                            !noisePattern.IsMatch(newCourse))
                        {
                            currentCourse = newCourse;
                            itemsSinceLastCourse = 0;
                        }
                    }
                    else
                    {
                        itemsSinceLastCourse++;
                        if (itemsSinceLastCourse > 5)
                        {
                            currentCourse = "";
                        }
                    }

                    if (string.IsNullOrWhiteSpace(currentCourse))
                        continue;

                    // ---- COLLECT DATA FROM CURRENT + FOLLOWING LINES ----
                    string dataText = line;
                    int startJ = i + 1 + skipLines;

                    for (int j = startJ; j < Math.Min(lines.Length, startJ + 5); j++)
                    {
                        string next = lines[j].Trim();
                        if (next.Contains("DOCENTE") || next.Contains("MODERADOR") || next == "CONTEUDISTA")
                            break;
                        dataText += " " + next;
                    }

                    // Remove category keywords to isolate numeric data
                    dataText = dataText
                        .Replace("DOCENTE CONTEUDISTA", " ")
                        .Replace("DOCENTE", " ")
                        .Replace("MODERADOR", " ")
                        .Trim();

                    // ---- EXTRACT NUMBERS ----
                    var nums = Regex.Matches(dataText, @"\d+");
                    if (nums.Count < 1) continue;

                    int qtd = int.Parse(nums[0].Value);
                    if (qtd <= 0 || qtd > 200) continue; // sanity check

                    decimal ch = nums.Count >= 2 ? decimal.Parse(nums[1].Value) : 0;

                    // ---- EXTRACT MONTH ----
                    var mesMatch = Regex.Match(dataText,
                        @"(JANEIRO|FEVEREIRO|MAR[ÇC]O|ABRIL|MAIO|JUNHO|JULHO|AGOSTO|SETEMBRO|OUTUBRO|NOVEMBRO|DEZEMBRO)",
                        RegexOptions.IgnoreCase);
                    string mes = mesMatch.Success ? mesMatch.Value.ToUpper() : "";

                    // ---- EXTRACT DATE ----
                    var dateMatch = Regex.Match(dataText, @"(\d{2}/\d{2}/\d{4})");
                    DateTime? data = null;
                    if (dateMatch.Success)
                    {
                        DateTime.TryParseExact(dateMatch.Value, "dd/MM/yyyy",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out var d);
                        if (d != default) data = d;
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

                    if (skipLines > 0)
                    {
                        i += skipLines;
                    }
                }
            }

            if (itens.Count == 0)
            {
                throw new TrilhasException("Nenhum item extraído do Anexo II. Verifique se o PDF contém a tabela com DOCENTE/MODERADOR e os cursos no formato esperado.");
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
        .ThenInclude(i => i.Slots)
    .Where(t => t.DeletionTime == null &&
                           (t.Status == "Aprovado" || t.Status == "Em Execução"))
                .ToList();

            foreach (var termo in termosAtivos)
            {
                foreach (var item in termo.Itens.Where(i => i.DeletionTime == null))
                {
                    if (item.ContratadosCount < item.Quantidade && item.DataOferta.HasValue)
                    {
                        if (item.DataOferta.Value > dataAtual && item.DataOferta.Value <= dataLimite)
                        {
                            var vagas = item.Quantidade - item.ContratadosCount;
                            
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