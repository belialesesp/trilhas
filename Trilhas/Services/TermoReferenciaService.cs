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
                // Reset stream position
                documentStream.Position = 0;

                // Extract data from PDF
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
                    throw new TrilhasException("Não foi possível extrair os profissionais necessários do documento. Verifique se o formato está correto.");
                }

                // Upload original document to storage
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

                // Save to database
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
                throw new TrilhasException($"Erro ao processar documento: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Extract title from PDF
        /// </summary>
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

                    // Look for title pattern
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

        /// <summary>
        /// Extract all data from PDF document
        /// </summary>
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
                
                // Extract text from all pages
                for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                {
                    var page = pdfDocument.GetPage(i);
                    var strategy = new SimpleTextExtractionStrategy();
                    string pageText = PdfTextExtractor.GetTextFromPage(page, strategy);
                    fullText += pageText + "\n";
                }

                // Extract demandante (from first page)
                var demandanteMatch = Regex.Match(fullText, @"Nome:\s*([^\n]+)", RegexOptions.IgnoreCase);
                if (demandanteMatch.Success)
                {
                    dados.Demandante = demandanteMatch.Groups[1].Value.Trim();
                }

                // Extract dates and duration
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

                // Extract items from Anexo II table
                dados.Itens = ExtrairItensDaTabela(fullText);
            }

            return dados;
        }

        /// <summary>
        /// Extract professional requirements from the table in Anexo II
        /// </summary>
        private List<TermoReferenciaItem> ExtrairItensDaTabela(string texto)
        {
            var itens = new List<TermoReferenciaItem>();

            // Find the Anexo II section
            var anexoMatch = Regex.Match(texto, @"Anexo II[^\n]*PLANILHA RESUMO DO PROJETO[^]*?(?=Observação:|$)", RegexOptions.Singleline);
            if (!anexoMatch.Success)
            {
                return itens;
            }

            string tabelaTexto = anexoMatch.Value;
            
            // Pattern to match table rows
            // Example: DIDÁTICA E ORATÓRIA PARA INSTRUTORES DO CBMES  DOCENTE  2  40  MARÇO  19/03/2026
            var linhaPattern = @"([A-ZÇÃÕÁÉÍÓÚÂÊÎÔÛ\s\-]+?)\s+(DOCENTE|MODERADOR|DOCENTE\s*CONTEUDISTA)\s+(\d+)\s+(\d+)\s+([A-ZÇÃÕÁÉÍÓÚ]+)\s+([\d\/]+)";
            var matches = Regex.Matches(tabelaTexto, linhaPattern, RegexOptions.Multiline);

            string cursoAtual = "";

            foreach (Match match in matches)
            {
                string cursoTexto = match.Groups[1].Value.Trim();
                string categoria = match.Groups[2].Value.Trim().Replace(" ", "_");
                int quantidade = int.Parse(match.Groups[3].Value);
                decimal cargaHoraria = decimal.Parse(match.Groups[4].Value);
                string mes = match.Groups[5].Value.Trim();
                string dataOferta = match.Groups[6].Value.Trim();

                // If curso is substantial, update current course
                if (cursoTexto.Length > 5 && !cursoTexto.StartsWith("DOCENTE"))
                {
                    cursoAtual = cursoTexto;
                }

                // Parse date (DD/MM/YYYY)
                DateTime? dataOfertaParsed = null;
                if (DateTime.TryParseExact(dataOferta, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                {
                    dataOfertaParsed = parsedDate;
                }

                var item = new TermoReferenciaItem
                {
                    Curso = cursoAtual,
                    Profissional = categoria,
                    Quantidade = quantidade,
                    CargaHoraria = cargaHoraria,
                    MesExecucao = mes,
                    DataOferta = dataOfertaParsed,
                    Contratados = 0,
                    CreatorUserId = "",
                    CreationTime = DateTime.Now
                };

                itens.Add(item);
            }

            return itens;
        }

        /// <summary>
        /// Check for courses starting within 15 days that need professionals
        /// </summary>
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
                    // Check if there are unfilled positions
                    if (item.Contratados < item.Quantidade && item.DataOferta.HasValue)
                    {
                        // Check if course starts within 15 days
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

        /// <summary>
        /// Send notifications to GEDTH users about upcoming courses
        /// </summary>
        public void EnviarNotificacoesContratacao()
        {
            var alertas = VerificarCursosProximos();
            
            if (!alertas.Any())
            {
                return;
            }

            // Get GEDTH users
            var gedthUsers = _context.Users
                .Join(_context.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { u, ur })
                .Join(_context.Roles, x => x.ur.RoleId, r => r.Id, (x, r) => new { x.u, r })
                .Where(x => x.r.Name == "GEDTH")
                .Select(x => x.u)
                .ToList();

            foreach (var user in gedthUsers)
            {
                // Send email
                _notificationService.EnviarEmailAlertaContratacao(user.Email, user.UserName, alertas);

                // Create in-app notification
                _notificationService.CriarNotificacaoInterna(user.Id, alertas);
            }
        }

        /// <summary>
        /// Update item's hired count
        /// </summary>
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