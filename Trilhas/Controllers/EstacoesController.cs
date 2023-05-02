using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Trilhas.Controllers.Mappers;
using Trilhas.Data.Model.Exceptions;
using Trilhas.Data.Model.Trilhas;
using Trilhas.Models.Trilhas.Estacao;
using Trilhas.Services;

namespace Trilhas.Controllers
{
    public class EstacoesController : DefaultController
    {
        private readonly EstacaoService _estacaoService;
        private readonly EixoService _eixoService;
        private readonly TrilhasService _service;
        private readonly EstacaoMapper _mapper;

        public EstacoesController(
            UserManager<IdentityUser> userManager,
            EstacaoService estacaoService,
            EixoService eixoService,
            TrilhasService service) : base(userManager)
        {
            _estacaoService = estacaoService;
            _eixoService = eixoService;
            _service = service;

            _mapper = new EstacaoMapper();
        }

        [HttpGet]
        public IActionResult Quantidade(string nome, long eixoId, bool excluidos)
        {
            int qtd = _estacaoService.QuantidadeDeEstacoes(nome, eixoId, excluidos);

            return new ObjectResult(qtd);
        }

        [HttpGet]
        public IActionResult Buscar(long eixoId, string nome, bool excluidos, int start, int count)
        {
            List<Estacao> estacoes = _estacaoService.PesquisarEstacoes(nome, eixoId, excluidos, start, count);

            var vm = _mapper.MapearEstacoesViewModel(estacoes);

            return Json(vm);
        }

        [HttpGet]
        public IActionResult DropDown(long eixoId)
        {
            var estacoes = _estacaoService.PesquisarEstacoes(null, eixoId, false);

            var vm = _mapper.MapearDropDownViewModel(estacoes);

            return Json(vm);
        }

        [HttpGet]
        public IActionResult Recuperar(long id)
        {
            Estacao estacao = _estacaoService.RecuperarEstacao(id);

            var vm = _mapper.MapearEstacaoViewModel(estacao);

            return Json(vm);
        }

        [HttpDelete]
        public IActionResult Excluir(long id)
        {
            try
            {
                _estacaoService.ExcluirEstacao(RecuperarUsuarioId(), id);
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
        public IActionResult Salvar([FromBody] SalvarEstacaoViewModel vm)
        {
            try
            {
                ValidarCadastroEstacao(vm);

                Estacao estacao;

                if (vm.Id > 0)
                {
                    estacao = AtualizarEstacao(vm);
                }
                else
                {
                    estacao = CriarEstacao(vm);
                }

                _estacaoService.SalvarEstacao(RecuperarUsuarioId(), estacao);

                return JsonFormResponse(estacao.Id);
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

        private Estacao CriarEstacao(SalvarEstacaoViewModel vm)
        {
            Eixo eixo = _eixoService.RecuperarEixo(vm.EixoId);

            return new Estacao
            {
                Eixo = eixo,
                Nome = vm.Nome.Trim(),
                Descricao = !string.IsNullOrEmpty(vm.Descricao) ? vm.Descricao.Trim() : null
            };
        }

        private Estacao AtualizarEstacao(SalvarEstacaoViewModel vm)
        {
            Estacao estacao = _estacaoService.RecuperarEstacao(vm.Id);
            estacao.Eixo = _eixoService.RecuperarEixo(vm.EixoId);
            estacao.Nome = vm.Nome.Trim();
            estacao.Descricao = !string.IsNullOrEmpty(vm.Descricao) ? vm.Descricao.Trim() : null;
            return estacao;
        }

        private void ValidarCadastroEstacao(SalvarEstacaoViewModel vm)
        {
            if (vm.EixoId <= 0)
            {
                ModelState.AddModelError("Eixo", "Selecione o Eixo.");
            }
            if (string.IsNullOrWhiteSpace(vm.Nome))
            {
                ModelState.AddModelError("Nome", "Preencha o nome da Estação.");
            }
            //if (string.IsNullOrWhiteSpace(vm.Descricao))
            //{
            //    ModelState.AddModelError("Descrição", "Preencha o campo Descrição.");
            //}

            if (!ModelState.IsValid)
            {
                throw new Exception("Preencha o formulário corretamente.");
            }
        }
    }
}
