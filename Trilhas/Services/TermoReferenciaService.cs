using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
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

        public TermoReferenciaService(ApplicationDbContext context, MinioService minioService)
        {
            _context = context;
            _minioService = minioService;
        }

        /// <summary>
        /// Processes an uploaded Word document and extracts Termo de Referência data
        /// </summary>
        public TermoDeReferencia ProcessarDocumento(string userId, Stream documentStream, string fileName, int ano)
        {
            var termo = new TermoDeReferencia
            {
                Titulo = Path.GetFileNameWithoutExtension(fileName),
                Ano = ano,
                Status = "Rascunho",
                CreatorUserId = userId,
                CreationTime = DateTime.Now
            };

            try
            {
                // Extract data from the document
                var itens = ExtrairItensDoDocumento(documentStream);
                
                if (itens == null || !itens.Any())
                {
                    throw new TrilhasException("Não foi possível extrair os itens do documento. Verifique se o formato está correto.");
                }

                termo.Itens = itens;

                // Upload original document to storage
                documentStream.Position = 0;
                
                // Convert Stream to MemoryStream if needed
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
                _minioService.SalvarImagemEixo(arquivo); // Using existing MinIO method
                termo.CaminhoArquivoOriginal = arquivo.Nome;

                // Save to database (EF will handle transaction automatically with EnableRetryOnFailure)
                _context.TermosDeReferencia.Add(termo);
                _context.SaveChanges();

                return termo;
            }
            catch (Exception ex)
            {
                // Log the inner exception but only throw with the message
                Console.WriteLine($"Error processing document: {ex.Message}");
                throw new TrilhasException($"Erro ao processar o documento: {ex.Message}");
            }
        }

        /// <summary>
        /// Extracts planning table data from Word document using OpenXML
        /// </summary>
        private List<TermoReferenciaItem> ExtrairItensDoDocumento(Stream documentStream)
        {
            var itens = new List<TermoReferenciaItem>();

            try
            {
                using (var wordDoc = WordprocessingDocument.Open(documentStream, false))
                {
                    var body = wordDoc.MainDocumentPart.Document.Body;
                    var tables = body.Descendants<Table>().ToList();

                    Console.WriteLine($"Total tables found: {tables.Count}");

                    // Find the table with headers matching our structure
                    Table targetTable = null;
                    foreach (var table in tables)
                    {
                        var firstRow = table.Elements<TableRow>().FirstOrDefault();
                        if (firstRow != null)
                        {
                            var cellTexts = firstRow.Elements<TableCell>()
                                .Select(c => GetCellText(c))
                                .ToList();

                            // Check if this is our planning table
                            if (cellTexts.Any(t => t.Contains("Curso")) && 
                                cellTexts.Any(t => t.Contains("Profissional")))
                            {
                                targetTable = table;
                                Console.WriteLine($"Found planning table with {table.Elements<TableRow>().Count()} rows");
                                break;
                            }
                        }
                    }

                    if (targetTable == null)
                    {
                        Console.WriteLine("Planning table not found!");
                        throw new TrilhasException("Tabela de planejamento não encontrada no documento.");
                    }

                    // Process data rows (skip header and "TOTAL" row)
                    var rows = targetTable.Elements<TableRow>().Skip(1).ToList();
                    
                    Console.WriteLine($"Processing {rows.Count} data rows");
                    
                    foreach (var row in rows)
                    {
                        var cells = row.Elements<TableCell>().Select(c => GetCellText(c)).ToList();
                        
                        // Skip empty rows or TOTAL row
                        if (cells.Count < 12)
                        {
                            Console.WriteLine($"Skipping row with only {cells.Count} cells");
                            continue;
                        }
                        
                        if (cells[0].Contains("TOTAL"))
                        {
                            Console.WriteLine("Skipping TOTAL row");
                            continue;
                        }

                        try
                        {
                        var item = new TermoReferenciaItem
                        {
                            Curso = cells.Count > 0 ? cells[0].Trim() : "",
                            Profissional = cells.Count > 1 ? cells[1].Trim() : "",
                            Quantidade = cells.Count > 2 ? ParseInt(cells[2]) : 0,
                            CargaHoraria = cells.Count > 3 ? ParseDecimal(cells[3]) : 0,
                            MesExecucao = cells.Count > 4 ? cells[4].Trim() : "",
                            DataOferta = cells.Count > 5 ? ParseDate(cells[5]) : null,
                            Modalidade = cells.Count > 6 ? cells[6].Trim() : "",
                            QuantidadeTurmas = cells.Count > 7 ? ParseInt(cells[7]) : 0,
                            AlunosPorTurma = cells.Count > 8 ? ParseInt(cells[8]) : 0,
                            ValorHora = cells.Count > 9 ? ParseDecimal(cells[9]) : 0,
                            EncargosPercentual = cells.Count > 10 ? ParseDecimal(cells[10]) : 0,
                            ValorTotal = cells.Count > 11 ? ParseDecimal(cells[11]) : 0,
                            CreationTime = DateTime.Now
                        };

                        // Calculate total if not provided
                        if (item.ValorTotal == 0 && item.Quantidade > 0)
                        {
                            item.ValorTotal = item.Quantidade * item.CargaHoraria * item.QuantidadeTurmas * 
                                             item.ValorHora * (1 + item.EncargosPercentual / 100);
                        }

                        // Only add if it has meaningful data (at least Profissional must be filled)
                        if (!string.IsNullOrWhiteSpace(item.Profissional) && 
                            item.Profissional.ToUpper() != "TOTAL")
                        {
                            // If Curso is empty, use a default name
                            if (string.IsNullOrWhiteSpace(item.Curso))
                            {
                                item.Curso = "Curso não especificado";
                            }
                            
                            itens.Add(item);
                        }
                        }
                        catch (Exception ex)
                        {
                            // Log but continue processing other rows
                            Console.WriteLine($"Erro ao processar linha: {ex.Message}");
                        }
                    }
                
                Console.WriteLine($"Successfully extracted {itens.Count} items from document");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting items from document: {ex.Message}");
                throw new TrilhasException($"Erro ao extrair itens do documento: {ex.Message}");
            }

            return itens;
        }
        

        private string GetCellText(TableCell cell)
        {
            return cell.InnerText.Trim();
        }

        private int ParseInt(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;
            value = Regex.Replace(value, @"[^\d]", "");
            return int.TryParse(value, out int result) ? result : 0;
        }

        private decimal ParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;
            
            // Remove currency symbols and clean up
            value = value.Replace("R$", "").Replace("$", "").Trim();
            
            // Handle both comma and dot as decimal separator
            value = value.Replace(".", "").Replace(",", ".");
            
            return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result) ? result : 0;
        }

        private DateTime? ParseDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            var formats = new[] { "dd/MM/yyyy", "dd/MM/yy", "yyyy-MM-dd" };
            
            if (DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }

            return null;
        }

        // CRUD Operations

        public List<TermoDeReferencia> PesquisarTermos(int? ano, string status, bool excluidos, int start = -1, int count = -1)
        {
            var query = _context.TermosDeReferencia
                .Include(t => t.Itens)
                .Where(t => (!excluidos && !t.DeletionTime.HasValue) || (excluidos && t.DeletionTime.HasValue));

            if (ano.HasValue)
            {
                query = query.Where(t => t.Ano == ano.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.Status == status);
            }

            query = query.OrderByDescending(t => t.Ano).ThenByDescending(t => t.CreationTime);

            if (start >= 0 && count > 0)
            {
                query = query.Skip(start).Take(count);
            }

            return query.ToList();
        }

        public int QuantidadeDeTermos(int? ano, string status, bool excluidos)
        {
            var query = _context.TermosDeReferencia
                .Where(t => (!excluidos && !t.DeletionTime.HasValue) || (excluidos && t.DeletionTime.HasValue));

            if (ano.HasValue)
            {
                query = query.Where(t => t.Ano == ano.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.Status == status);
            }

            return query.Count();
        }

        public TermoDeReferencia RecuperarTermo(long id, bool incluirExcluidos)
        {
            var query = _context.TermosDeReferencia
                .Include(t => t.Itens.Where(i => !i.DeletionTime.HasValue))
                .Where(t => t.Id == id);

            if (!incluirExcluidos)
            {
                query = query.Where(t => !t.DeletionTime.HasValue);
            }

            return query.FirstOrDefault();
        }

        public TermoDeReferencia SalvarTermo(string userId, TermoDeReferencia termo)
        {
            if (termo.Id > 0)
            {
                var existing = RecuperarTermo(termo.Id, true);
                if (existing == null)
                {
                    throw new RecordNotFoundException("Termo de Referência não encontrado.");
                }

                existing.Titulo = termo.Titulo;
                existing.Descricao = termo.Descricao;
                existing.Status = termo.Status;
                existing.DataAprovacao = termo.DataAprovacao;
                existing.LastModifierUserId = userId;
                existing.LastModificationTime = DateTime.Now;
            }
            else
            {
                termo.CreatorUserId = userId;
                termo.CreationTime = DateTime.Now;
                _context.TermosDeReferencia.Add(termo);
            }

            _context.SaveChanges();

            return termo;
        }

        public void SalvarItem(string userId, TermoReferenciaItem item)
        {
            if (item.Id > 0)
            {
                var existing = _context.TermoReferenciaItens.Find(item.Id);
                if (existing != null)
                {
                    existing.Curso = item.Curso;
                    existing.Profissional = item.Profissional;
                    existing.Quantidade = item.Quantidade;
                    existing.CargaHoraria = item.CargaHoraria;
                    existing.MesExecucao = item.MesExecucao;
                    existing.DataOferta = item.DataOferta;
                    existing.Modalidade = item.Modalidade;
                    existing.QuantidadeTurmas = item.QuantidadeTurmas;
                    existing.AlunosPorTurma = item.AlunosPorTurma;
                    existing.ValorHora = item.ValorHora;
                    existing.EncargosPercentual = item.EncargosPercentual;
                    existing.ValorTotal = item.ValorTotal;
                    existing.LastModifierUserId = userId;
                    existing.LastModificationTime = DateTime.Now;
                }
            }
            else
            {
                item.CreatorUserId = userId;
                item.CreationTime = DateTime.Now;
                _context.TermoReferenciaItens.Add(item);
            }

            _context.SaveChanges();
        }

        public void ExcluirTermo(string userId, long id)
        {
            var termo = RecuperarTermo(id, false);
            
            if (termo == null)
            {
                throw new RecordNotFoundException("Termo de Referência não encontrado.");
            }

            termo.DeletionUserId = userId;
            termo.DeletionTime = DateTime.Now;

            _context.SaveChanges();
        }

        public void ExcluirItem(string userId, long itemId)
        {
            var item = _context.TermoReferenciaItens.Find(itemId);
            
            if (item == null || item.DeletionTime.HasValue)
            {
                throw new RecordNotFoundException("Item não encontrado.");
            }

            item.DeletionUserId = userId;
            item.DeletionTime = DateTime.Now;

            _context.SaveChanges();
        }
    }
}