using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Trilhas.Data.Model.Exceptions;
using Trilhas.Data.Model.TermosReferencia;
using Trilhas.Models.TermosReferencia;
using Trilhas.Services;

namespace Trilhas.Controllers
{
    [Authorize(Roles = "GEDTH,Administrador")]
    public class TermosReferenciaController : DefaultController
    {
        private readonly TermoReferenciaService _service;

        public TermosReferenciaController(
            UserManager<IdentityUser> userManager,
            TermoReferenciaService service) : base(userManager)
        {
            _service = service;
        }

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
        /// Upload and process a Word document
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

                if (!file.FileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { message = "Apenas arquivos .docx são aceitos." });
                }

                if (ano < 2020 || ano > 2050)
                {
                    return BadRequest(new { message = "Ano inválido." });
                }

                using (var stream = file.OpenReadStream())
                {
                    var termo = _service.ProcessarDocumento(
                        RecuperarUsuarioId(), 
                        stream, 
                        file.FileName, 
                        ano
                    );

                    return Ok(new 
                    { 
                        message = "Documento processado com sucesso!",
                        termoId = termo.Id,
                        totalItens = termo.Itens.Count
                    });
                }
            }
            catch (TrilhasException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao processar o documento: " + ex.Message });
            }
        }

        /// <summary>
        /// Get count of Termos de Referência
        /// </summary>
        [HttpGet]
        public IActionResult Quantidade(int? ano, string status, bool excluidos)
        {
            int qtd = _service.QuantidadeDeTermos(ano, status, excluidos);
            return Ok(qtd);
        }

        /// <summary>
        /// Search Termos de Referência with pagination
        /// </summary>
        [HttpGet]
        public IActionResult Buscar(int? ano, string status, bool excluidos, int start = -1, int count = -1)
        {
            var termos = _service.PesquisarTermos(ano, status, excluidos, start, count);
            
            var result = termos.Select(t => new
            {
                id = t.Id,
                titulo = t.Titulo,
                ano = t.Ano,
                numeroDocumento = t.NumeroDocumento,
                status = t.Status,
                descricao = t.Descricao,
                dataCriacao = t.CreationTime.ToString("dd/MM/yyyy"),
                dataAprovacao = t.DataAprovacao?.ToString("dd/MM/yyyy"),
                totalItens = t.Itens?.Count ?? 0,
                valorTotal = t.Itens?.Sum(i => i.ValorTotal) ?? 0
            });

            return Json(result);
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
                        valorTotal = i.ValorTotal
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
        /// Update Termo metadata (title, description, status)
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
                termo.Descricao = vm.Descricao;
                termo.Status = vm.Status;
                
                if (!string.IsNullOrEmpty(vm.DataAprovacao))
                {
                    if (DateTime.TryParse(vm.DataAprovacao, out DateTime dataAprov))
                    {
                        termo.DataAprovacao = dataAprov;
                    }
                }

                _service.SalvarTermo(RecuperarUsuarioId(), termo);

                return Ok(new { message = "Termo atualizado com sucesso!" });
            }
            catch (RecordNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update a specific item in the planning table
        /// </summary>
        [HttpPut]
        public IActionResult AtualizarItem([FromBody] TermoReferenciaItemViewModel vm)
        {
            try
            {
                var item = new TermoReferenciaItem
                {
                    Id = vm.Id,
                    TermoDeReferenciaId = vm.TermoDeReferenciaId,
                    Curso = vm.Curso,
                    Profissional = vm.Profissional,
                    Quantidade = vm.Quantidade,
                    CargaHoraria = vm.CargaHoraria,
                    MesExecucao = vm.MesExecucao,
                    Modalidade = vm.Modalidade,
                    QuantidadeTurmas = vm.QuantidadeTurmas,
                    AlunosPorTurma = vm.AlunosPorTurma,
                    ValorHora = vm.ValorHora,
                    EncargosPercentual = vm.EncargosPercentual
                };

                if (!string.IsNullOrEmpty(vm.DataOferta))
                {
                    if (DateTime.TryParse(vm.DataOferta, out DateTime data))
                    {
                        item.DataOferta = data;
                    }
                }

                // Calculate total
                item.ValorTotal = item.Quantidade * item.CargaHoraria * item.QuantidadeTurmas * 
                                 item.ValorHora * (1 + item.EncargosPercentual / 100);

                _service.SalvarItem(RecuperarUsuarioId(), item);

                return Ok(new 
                { 
                    message = "Item atualizado com sucesso!",
                    valorTotal = item.ValorTotal
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Add a new item to the planning table
        /// </summary>
        [HttpPost]
        public IActionResult AdicionarItem([FromBody] TermoReferenciaItemViewModel vm)
        {
            try
            {
                var item = new TermoReferenciaItem
                {
                    TermoDeReferenciaId = vm.TermoDeReferenciaId,
                    Curso = vm.Curso,
                    Profissional = vm.Profissional,
                    Quantidade = vm.Quantidade,
                    CargaHoraria = vm.CargaHoraria,
                    MesExecucao = vm.MesExecucao,
                    Modalidade = vm.Modalidade,
                    QuantidadeTurmas = vm.QuantidadeTurmas,
                    AlunosPorTurma = vm.AlunosPorTurma,
                    ValorHora = vm.ValorHora,
                    EncargosPercentual = vm.EncargosPercentual
                };

                if (!string.IsNullOrEmpty(vm.DataOferta))
                {
                    if (DateTime.TryParse(vm.DataOferta, out DateTime data))
                    {
                        item.DataOferta = data;
                    }
                }

                // Calculate total
                item.ValorTotal = item.Quantidade * item.CargaHoraria * item.QuantidadeTurmas * 
                                 item.ValorHora * (1 + item.EncargosPercentual / 100);

                _service.SalvarItem(RecuperarUsuarioId(), item);

                return Ok(new 
                { 
                    message = "Item adicionado com sucesso!",
                    itemId = item.Id,
                    valorTotal = item.ValorTotal
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Soft delete a Termo de Referência
        /// </summary>
        [HttpDelete]
        public IActionResult Excluir(long id)
        {
            try
            {
                _service.ExcluirTermo(RecuperarUsuarioId(), id);
                return Ok(new { message = "Termo excluído com sucesso!" });
            }
            catch (RecordNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Soft delete an item from the planning table
        /// </summary>
        [HttpDelete]
        public IActionResult ExcluirItem(long itemId)
        {
            try
            {
                _service.ExcluirItem(RecuperarUsuarioId(), itemId);
                return Ok(new { message = "Item excluído com sucesso!" });
            }
            catch (RecordNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}