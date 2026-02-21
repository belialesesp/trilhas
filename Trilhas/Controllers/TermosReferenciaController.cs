using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Trilhas.Data;
using Trilhas.Data.Model.Exceptions;
using Trilhas.Data.Model.TermosReferencia;
using Trilhas.Models.TermosReferencia;
using Trilhas.Services;
using Microsoft.EntityFrameworkCore;

namespace Trilhas.Controllers
{
    [Authorize(Roles = "GEDTH,Coordenador,Administrador")]
    public class TermosReferenciaController : DefaultController
    {
        private readonly TermoReferenciaService _service;


        /// <summary>
        /// Main view for managing Termos de Referência
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// View to display details and items of a specific Termo
        /// </summary>
        [HttpGet]
        public IActionResult Detalhes(long id)
        {
            var termo = _service.RecuperarTermo(id, false);
            
            if (termo == null)
            {
                return NotFound();
            }

            ViewBag.Termo = termo;
            ViewBag.TermoId = id;
            return View();
        }

        /// <summary>
        /// Upload and process a PDF document
        /// </summary>
        [HttpPost]
        public IActionResult Upload(IFormFile file, int ano)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "Nenhum arquivo foi enviado." });
                }

                // Accept both PDF and DOCX
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (extension != ".pdf" && extension != ".docx")
                {
                    return BadRequest(new { message = "Apenas arquivos .pdf ou .docx são aceitos." });
                }

                if (ano < 2020 || ano > 2050)
                {
                    return BadRequest(new { message = "Ano inválido." });
                }

                TermoDeReferencia termo;

                using (var stream = file.OpenReadStream())
                {
                    if (extension == ".pdf")
                    {
                        termo = _service.ProcessarDocumentoPDF(
                            RecuperarUsuarioId(), 
                            stream, 
                            file.FileName, 
                            ano
                        );
                    }
                    else
                    {
                        // Keep existing DOCX processing if you have it
                        return BadRequest(new { message = "Processamento de .docx ainda não implementado. Use .pdf" });
                    }
                }

                return Ok(new 
                { 
                    message = "Documento processado com sucesso!", 
                    termoId = termo.Id,
                    itensExtraidos = termo.Itens.Count
                });
            }
            catch (TrilhasException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao processar documento: {ex.Message}" });
            }
        }

        /// <summary>
        /// Search for Termos de Referência with filters
        /// </summary>
        [HttpGet]
        public IActionResult Buscar(int? ano, string status, bool excluidos = false, int start = 0, int count = 10)
        {
            try
            {
                var termos = _service.BuscarTermos(ano, status, excluidos, start, count);

                var result = termos.Select(t => new
                {
                    id = t.Id,
                    titulo = t.Titulo,
                    demandante = t.Demandante,
                    ano = t.Ano,
                    status = t.Status,
                    dataCriacao = t.CreationTime.ToString("dd/MM/yyyy"),
                    dataAprovacao = t.DataAprovacao?.ToString("dd/MM/yyyy"),
                    numeroItens = t.Itens?.Count ?? 0,
                    valorTotal = t.Itens?.Sum(i => i.ValorTotal) ?? 0
                });

                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get a specific Termo de Referência with all its items
        /// </summary>
        [HttpGet]
        public IActionResult Recuperar(long id)
        {
            try
            {
                var termo = _service.RecuperarTermo(id, false);
                
                if (termo == null)
                {
                    return NotFound(new { message = "Termo de Referência não encontrado." });
                }

                var result = new
                {
                    id = termo.Id,
                    titulo = termo.Titulo,
                    ano = termo.Ano,
                    numeroDocumento = termo.NumeroDocumento,
                    descricao = termo.Descricao,
                    demandante = termo.Demandante,
                    dataInicio = termo.DataInicio,
                    dataTermino = termo.DataTermino,
                    duracao = termo.Duracao,
                    status = termo.Status,
                    dataCriacao = termo.CreationTime.ToString("dd/MM/yyyy"),
                    dataAprovacao = termo.DataAprovacao?.ToString("dd/MM/yyyy"),
                    caminhoArquivo = termo.CaminhoArquivoOriginal,
                    itens = termo.Itens.Select(i => new
                    {
                        id = i.Id,
                        curso = i.Curso,
                        profissional = i.Profissional,
                        quantidade = i.Quantidade,
                        cargaHoraria = i.CargaHoraria,
                        mesExecucao = i.MesExecucao,
                        dataOferta = i.DataOferta?.ToString("dd/MM/yyyy"),
                        modalidade = i.Modalidade,
                        quantidadeTurmas = i.QuantidadeTurmas,
                        alunosPorTurma = i.AlunosPorTurma,
                        valorHora = i.ValorHora,
                        encargosPercentual = i.EncargosPercentual,
                        valorTotal = i.ValorTotal,
                        contratados = i.Contratados,
                        vagasRestantes = i.Quantidade - i.Contratados
                    }).ToList()
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update Termo metadata
        /// </summary>
        [HttpPut]
        public IActionResult Atualizar([FromBody] TermoReferenciaViewModel vm)
        {
            try
            {
                var termo = _service.RecuperarTermo(vm.Id, false);
                
                if (termo == null)
                {
                    return NotFound(new { message = "Termo de Referência não encontrado." });
                }

                termo.Titulo = vm.Titulo;
                termo.NumeroDocumento = vm.NumeroDocumento;
                termo.Descricao = vm.Descricao;
                termo.Demandante = vm.Demandante;
                termo.DataInicio = vm.DataInicio;
                termo.DataTermino = vm.DataTermino;
                termo.Duracao = vm.Duracao;
                termo.Status = vm.Status;
                termo.LastModifierUserId = RecuperarUsuarioId();
                termo.LastModificationTime = DateTime.Now;

                _service.SalvarAlteracoes();

                return Ok(new { message = "Termo atualizado com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update an item's data
        /// </summary>
        [HttpPut]
        public IActionResult AtualizarItem([FromBody] TermoReferenciaItemViewModel vm)
        {
            try
            {
                var item = _service.RecuperarItem(vm.Id);
                
                if (item == null)
                {
                    return NotFound(new { message = "Item não encontrado." });
                }

                item.Curso = vm.Curso;
                item.Profissional = vm.Profissional;
                item.Quantidade = vm.Quantidade;
                item.CargaHoraria = vm.CargaHoraria;
                item.MesExecucao = vm.MesExecucao;
                
                if (!string.IsNullOrEmpty(vm.DataOferta))
                {
                    item.DataOferta = DateTime.ParseExact(vm.DataOferta, "dd/MM/yyyy", null);
                }

                item.Modalidade = vm.Modalidade;
                item.QuantidadeTurmas = vm.QuantidadeTurmas;
                item.AlunosPorTurma = vm.AlunosPorTurma;
                item.ValorHora = vm.ValorHora;
                item.EncargosPercentual = vm.EncargosPercentual;
                item.ValorTotal = vm.ValorTotal;
                item.LastModifierUserId = RecuperarUsuarioId();
                item.LastModificationTime = DateTime.Now;

                _service.SalvarAlteracoes();

                return Ok(new { message = "Item atualizado com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update the number of hired professionals for an item
        /// </summary>
        [HttpPost]
        public IActionResult AtualizarContratados(long itemId, int quantidade)
        {
            try
            {
                _service.AtualizarContratados(itemId, quantidade);
                return Ok(new { message = "Quantidade de contratados atualizada!" });
            }
            catch (TrilhasException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get alerts for courses starting soon that need staff
        /// </summary>
        [HttpGet]
        public IActionResult ObterAlertas()
        {
            try
            {
                var alertas = _service.VerificarCursosProximos();

                var result = alertas.Select(a => new
                {
                    termoId = a.TermoId,
                    termoTitulo = a.TermoTitulo,
                    demandante = a.Demandante,
                    curso = a.Curso,
                    categoria = a.Categoria,
                    vagasRestantes = a.VagasRestantes,
                    dataOferta = a.DataOferta.ToString("dd/MM/yyyy"),
                    diasRestantes = a.DiasRestantes
                });

                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Manually trigger notification sending
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult EnviarNotificacoes()
        {
            try
            {
                _service.EnviarNotificacoesContratacao();
                return Ok(new { message = "Notificações enviadas com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete (soft) a Termo de Referência
        /// </summary>
        [HttpDelete]
        public IActionResult Excluir(long id)
        {
            try
            {
                _service.ExcluirTermo(id, RecuperarUsuarioId());
                return Ok(new { message = "Termo excluído com sucesso!" });
            }
            catch (TrilhasException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        private readonly ApplicationDbContext _context;

public TermosReferenciaController(
    UserManager<IdentityUser> userManager,
    TermoReferenciaService service,
    ApplicationDbContext context) : base(userManager)  // <-- ADD context parameter
{
    _service = service;
    _context = context;  // <-- STORE IT
}
/// <summary>
        /// Save contractor slot information
        /// ADD THIS TO TermosReferenciaController.cs
        /// </summary>
        [HttpPost]
        public IActionResult SalvarSlot(long slotId, string nomeContratado, bool ateste)
        {
            try
            {
                var slot = _context.ContratadoSlots.Find(slotId);
                
                if (slot == null)
                {
                    return Json(new { success = false, message = "Slot não encontrado." });
                }

                var userId = _userManager.GetUserId(User);
                
                slot.NomeContratado = nomeContratado;
                slot.Ateste = ateste;
                slot.LastModificationTime = DateTime.Now;
                
                // If filling the contractor name for the first time, record it
                if (!string.IsNullOrWhiteSpace(nomeContratado) && !slot.DataContratacao.HasValue)
                {
                    slot.DataContratacao = DateTime.Now;
                    slot.PreenchidoPorUserId = userId;
                }
                
                _context.SaveChanges();
                
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        /// <summary>
        /// GET: Create new Termo manually
        /// ADD THESE TO TermosReferenciaController.cs
        /// </summary>
        [HttpGet]
        public IActionResult Criar()
        {
            ViewBag.AnoAtual = DateTime.Now.Year;
            return View();
        }

        /// <summary>
        /// POST: Save manually created Termo
        /// </summary>
        [HttpPost]
        public IActionResult Criar(string titulo, string demandante, int ano, string dataInicio, string dataTermino)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                
                var termo = new TermoDeReferencia
                {
                    Titulo = titulo,
                    Demandante = demandante,
                    Ano = ano,
                    DataInicio = dataInicio,
                    DataTermino = dataTermino,
                    Status = "Rascunho",
                    CreatorUserId = userId,
                    CreationTime = DateTime.Now
                };

                _context.TermosDeReferencia.Add(termo);
                _context.SaveChanges();

                return Json(new { success = true, termoId = termo.Id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// GET: Edit Termo basic info
        /// </summary>
        [HttpGet]
        public IActionResult Editar(long id)
        {
            var termo = _service.RecuperarTermo(id, false);
            
            if (termo == null)
            {
                return NotFound();
            }

            return View(termo);
        }

        /// <summary>
        /// POST: Update Termo basic info
        /// </summary>
        [HttpPost]
        public IActionResult AtualizarTermo(long id, string titulo, string demandante, int ano, 
            string dataInicio, string dataTermino, string status)
        {
            try
            {
                var termo = _context.TermosDeReferencia.Find(id);
                
                if (termo == null)
                {
                    return Json(new { success = false, message = "Termo não encontrado." });
                }

                termo.Titulo = titulo;
                termo.Demandante = demandante;
                termo.Ano = ano;
                termo.DataInicio = dataInicio;
                termo.DataTermino = dataTermino;
                termo.Status = status;
                termo.LastModificationTime = DateTime.Now;

                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// POST: Add new course item to Termo
        /// </summary>
        [HttpPost]
        public IActionResult AdicionarItem(long termoId, string curso, string profissional, 
            int quantidade, decimal cargaHoraria, string mesExecucao, string dataOferta)
        {
            try
            {
                var termo = _context.TermosDeReferencia.Find(termoId);
                
                if (termo == null)
                {
                    return Json(new { success = false, message = "Termo não encontrado." });
                }

                DateTime? dataOfertaParsed = null;
                if (!string.IsNullOrWhiteSpace(dataOferta))
                {
                    DateTime.TryParseExact(dataOferta, "yyyy-MM-dd", 
                        System.Globalization.CultureInfo.InvariantCulture, 
                        System.Globalization.DateTimeStyles.None, out var parsed);
                    dataOfertaParsed = parsed;
                }

                var item = new TermoReferenciaItem
                {
                    TermoDeReferenciaId = termoId,
                    Curso = curso,
                    Profissional = profissional,
                    Quantidade = quantidade,
                    CargaHoraria = cargaHoraria,
                    MesExecucao = mesExecucao,
                    DataOferta = dataOfertaParsed,
                    Contratados = 0,
                    CreatorUserId = _userManager.GetUserId(User),
                    CreationTime = DateTime.Now
                };

                // Create slots for this item
                for (int i = 1; i <= quantidade; i++)
                {
                    item.Slots.Add(new ContratadoSlot
                    {
                        NumeroSlot = i,
                        NomeContratado = null,
                        Ateste = false,
                        CreationTime = DateTime.Now
                    });
                }

                _context.TermoReferenciaItens.Add(item);
                _context.SaveChanges();

                return Json(new { success = true, itemId = item.Id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// POST: Update existing item
        /// </summary>
        [HttpPost]
        public IActionResult AtualizarItem(long itemId, string curso, string profissional, 
            int quantidade, decimal cargaHoraria, string mesExecucao, string dataOferta)
        {
            try
            {
                var item = _context.TermoReferenciaItens
                    .Include(i => i.Slots)
                    .FirstOrDefault(i => i.Id == itemId);
                
                if (item == null)
                {
                    return Json(new { success = false, message = "Item não encontrado." });
                }

                item.Curso = curso;
                item.Profissional = profissional;
                item.CargaHoraria = cargaHoraria;
                item.MesExecucao = mesExecucao;
                
                if (!string.IsNullOrWhiteSpace(dataOferta))
                {
                    DateTime.TryParseExact(dataOferta, "yyyy-MM-dd", 
                        System.Globalization.CultureInfo.InvariantCulture, 
                        System.Globalization.DateTimeStyles.None, out var parsed);
                    item.DataOferta = parsed;
                }

                // Adjust slots if quantity changed
                int currentSlots = item.Slots.Count;
                
                if (quantidade > currentSlots)
                {
                    // Add more slots
                    for (int i = currentSlots + 1; i <= quantidade; i++)
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
                else if (quantidade < currentSlots)
                {
                    // Remove excess slots (from the end, keep filled ones)
                    var slotsToRemove = item.Slots
                        .OrderByDescending(s => s.NumeroSlot)
                        .Take(currentSlots - quantidade)
                        .Where(s => string.IsNullOrWhiteSpace(s.NomeContratado))
                        .ToList();
                    
                    foreach (var slot in slotsToRemove)
                    {
                        _context.ContratadoSlots.Remove(slot);
                    }
                }

                item.Quantidade = quantidade;
                item.LastModificationTime = DateTime.Now;

                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// POST: Delete item
        /// </summary>
        [HttpPost]
        public IActionResult DeletarItem(long itemId)
        {
            try
            {
                var item = _context.TermoReferenciaItens.Find(itemId);
                
                if (item == null)
                {
                    return Json(new { success = false, message = "Item não encontrado." });
                }

                // Soft delete
                item.DeletionTime = DateTime.Now;
                item.DeletionUserId = _userManager.GetUserId(User);
                
                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
    
    
}
