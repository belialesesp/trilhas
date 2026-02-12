using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Trilhas.Controllers.Mappers;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Data.Model.Exceptions;
using Trilhas.Models.Cadastros.Recurso;
using Trilhas.Services;

namespace Trilhas.Controllers
{
    [Authorize(Roles = "Administrador,GESE,GEDTH")]
    public class RecursosController : DefaultController
    {
        private readonly RecursoService _recursoService;
        private readonly RecursoMapper _mapper;

        public RecursosController(UserManager<IdentityUser> userManager, RecursoService recursoService) : base(userManager)
        {
            _recursoService = recursoService;
            _mapper = new RecursoMapper();
        }

        [HttpGet]
        public IActionResult Quantidade(string nome, string descricao, bool excluidos)
        {
            int qtd = _recursoService.QuantidadeDeRecursos(nome, descricao, excluidos);
            return new ObjectResult(qtd);
        }

        [HttpGet]
        public IActionResult Buscar(string nome, string descricao, bool excluidos, int start = -1, int count = -1)
        {
            List<Recurso> recursos = _recursoService.PesquisarRecursos(nome, descricao, excluidos, start, count);

            var vm = _mapper.MapearRecursosViewModel(recursos);

            return Json(vm);
        }

        [HttpGet]
        public IActionResult Recuperar(long id)
        {
            Recurso recurso = _recursoService.RecuperarRecurso(id, true);

            var vm = _mapper.MapearRecursoViewModel(recurso);

            return Json(vm);
        }

        [HttpDelete]
        public IActionResult Excluir(long id)
        {
            try
            {
                _recursoService.ExcluirRecurso(RecuperarUsuarioId(), id);
            }
            catch (RecordNotFoundException rex)
            {
                return BadRequest(rex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return new EmptyResult();
        }

        [HttpPost]
        public IActionResult Salvar([FromBody] SalvarRecursoViewModel vm)
        {
            try
            {
                ValidarCadastroRecurso(vm);

                Recurso recurso;

                if (vm.Id > 0)
                {
                    recurso = AtualizarRecurso(vm);
                }
                else
                {
                    recurso = CriarRecurso(vm);
                }

                recurso = _recursoService.SalvarRecurso(RecuperarUsuarioId(), recurso);

                return JsonFormResponse(recurso.Id);
            }
            catch (TrilhasException tex)
            {
                return JsonErrorFormResponse(tex);
            }
            catch (Exception ex)
            {
                return JsonErrorFormResponse(ex, "Ocorreu um erro ao salvar o registro.");
            }
        }

        private Recurso CriarRecurso(SalvarRecursoViewModel vm)
        {
            return new Recurso
            {
                Nome = vm.Nome.Trim(),
                Descricao = !string.IsNullOrWhiteSpace(vm.Descricao) ? vm.Descricao.Trim() : "",
                Custo = vm.Custo
            };
        }

        private Recurso AtualizarRecurso(SalvarRecursoViewModel vm)
        {
            Recurso recurso = _recursoService.RecuperarRecurso(vm.Id);

            recurso.Nome = vm.Nome.Trim();
            recurso.Descricao = !string.IsNullOrWhiteSpace(vm.Descricao) ? vm.Descricao.Trim() : "";
            recurso.Custo = vm.Custo;
            return recurso;
        }

        private void ValidarCadastroRecurso(SalvarRecursoViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Nome))
            {
                ModelState.AddModelError("Nome", "Informe o Nome do Recurso.");
            }
            //if (string.IsNullOrWhiteSpace(vm.Descricao))
            //{
            //    ModelState.AddModelError("Descrição", "Informe a Descrição do Recurso.");
            //}

            if (!ModelState.IsValid)
            {
                throw new Exception("Preencha o formulário corretamente.");
            }
        }
    }
}