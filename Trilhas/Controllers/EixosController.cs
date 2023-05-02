using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using Trilhas.Controllers.Mappers;
using Trilhas.Data.Model.Exceptions;
using Trilhas.Data.Model.Trilhas;
using Trilhas.Models.Trilhas.Eixo;
using Trilhas.Services;

namespace Trilhas.Controllers
{
    public class EixosController : DefaultController
    {
        private readonly EixoService _service;
        private readonly EstacaoService _estacaoService;
        private readonly EixoMapper _mapper;

        public EixosController(
            UserManager<IdentityUser> userManager,
            EstacaoService estacaoService,
            EixoService eixoService,
            TrilhasService service) : base(userManager)
        {
            _estacaoService = estacaoService;
            _service = eixoService;

            _mapper = new EixoMapper();
        }

        [HttpGet]
        public IActionResult Quantidade(string nome, bool excluidos)
        {
            int qtd = _service.QuantidadeDeEixos(nome, excluidos);

            return new ObjectResult(qtd);
        }

        [HttpGet]
        public IActionResult Buscar(string nome, bool excluidos, int start = -1, int count = -1)
        {
            var eixos = _service.PesquisarEixos(nome, excluidos, start, count);

            var vm = _mapper.MapearEixosViewModel(eixos);

            return Json(vm);
        }

        [HttpGet]
        public IActionResult DropDown()
        {
            var eixos = _service.PesquisarEixos(null, false);

            var vm = _mapper.MapearDropDownViewModel(eixos);

            return Json(vm);
        }

        [HttpGet]
        public IActionResult Recuperar(long id)
        {
            var eixo = _service.RecuperarEixo(id, true);

            var vm = _mapper.MapearEixoViewModel(eixo);

            return Json(vm);
        }

        [HttpDelete]
        public IActionResult Excluir(long id)
        {
            try
            {
                _service.ExcluirEixo(RecuperarUsuarioId(), id);
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
        public IActionResult Salvar([FromBody] SalvarEixoViewModel vm)
        {
            try
            {
                ValidarCadastroEixo(vm);

                Eixo eixo;

                if (vm.Id > 0)
                {
                    eixo = AtualizarEixo(vm);
                }
                else
                {
                    eixo = CriarEixo(vm);
                }

                eixo = _service.SalvarEixo(RecuperarUsuarioId(), eixo);

                return JsonFormResponse(eixo.Id);
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

        private Eixo CriarEixo(SalvarEixoViewModel vm)
        {
            return new Eixo
            {
                Nome = vm.Nome.Trim(),
                Descricao = !string.IsNullOrEmpty(vm.Descricao) ? vm.Descricao.Trim() : null,
                Imagem = vm.Imagem
            };
    }

    private Eixo AtualizarEixo(SalvarEixoViewModel vm)
    {
        var eixo = _service.RecuperarEixo(vm.Id);
        eixo.Nome = vm.Nome.Trim();
        eixo.Descricao = !string.IsNullOrEmpty(vm.Descricao) ? vm.Descricao.Trim() : null;
        eixo.Imagem = vm.Imagem;
        return eixo;
    }

    private void ValidarCadastroEixo(SalvarEixoViewModel vm)
    {
        if (string.IsNullOrWhiteSpace(vm.Nome))
        {
            ModelState.AddModelError("Nome", "Preencha o nome do Eixo.");
        }
        //if (string.IsNullOrWhiteSpace(vm.Descricao))
        //{
        //    ModelState.AddModelError("Descrição", "Preencha a Descrição.");
        //}

        if (!ModelState.IsValid)
        {
            throw new Exception("Preencha o formulário corretamente.");
        }
    }
}
}