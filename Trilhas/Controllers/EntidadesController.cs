using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Trilhas.Controllers.Mappers;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Data.Model.Exceptions;
using Trilhas.Models.Cadastros;
using Trilhas.Services;

namespace Trilhas.Controllers
{
	public class EntidadesController : DefaultController
    {
        private readonly EntidadeService _entidadeService;
        private readonly CadastroService _cadastroService;
        private readonly PessoaService _pessoaService;
        private readonly EntidadeMapper _mapper;

        public EntidadesController(UserManager<IdentityUser> userManager,
            CadastroService cadastroService,
            EntidadeService entidadeService,
            PessoaService pessoaService) : base(userManager)
        {
            _entidadeService = entidadeService;
            _cadastroService = cadastroService;
            _pessoaService = pessoaService;
            _mapper = new EntidadeMapper();
        }

        [HttpGet]
        public IActionResult Quantidade(string nome, long tipoEntidadeId, string uf, long municipioId, bool excluidos)
        {
            int qtd = _entidadeService.QuantidadeDeEntidades(nome, tipoEntidadeId, uf, municipioId, excluidos);
            return new ObjectResult(qtd);
        }

        [HttpGet]
        public IActionResult Buscar(string nome, long tipoEntidadeId, string uf, long municipioId, bool excluidos, int start = -1, int count = -1)
        {
            List<Entidade> entidades = _entidadeService.PesquisarEntidades(nome, tipoEntidadeId, uf, municipioId, excluidos, start, count);

            var vm = _mapper.MapearEntidadesViewModel(entidades);

            return Json(vm);
        }

        [HttpGet]
        public IActionResult Recuperar(long id)
        {
            Entidade entidade = _entidadeService.RecuperarEntidadeCompleta(id, true);

            var vm = _mapper.MapearEntidadeViewModel(entidade);

            return Json(vm);
        }

        [HttpGet]
        public IActionResult RecuperarBasico(long id)
        {
            Entidade entidade = _entidadeService.RecuperarEntidade(id, true);

            var vm = _mapper.MapearEntidadeBasicaViewModel(entidade);

            return Json(vm);
        }

        [HttpGet]
        public IActionResult RecuperarTipos()
        {
            List<TipoDeEntidade> tipos = _entidadeService.RecuperarTipoEntidade();

            var vm = _mapper.MapearTipoEntidadeViewModel(tipos);

            return Json(vm);
        }

        [HttpDelete]
        public IActionResult Excluir(long id)
        {
            try
            {
                _entidadeService.ExcluirEntidade(RecuperarUsuarioId(), id);
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
        public IActionResult Salvar([FromBody] SalvarEntidadeViewModel vm)
        {
            try
            {
                ValidarCadastroEntidade(vm);

                Entidade entidade;

                if (vm.Id > 0)
                {
                    entidade = AtualizarEntidade(vm);
                }
                else
                {
                    entidade = CriarEntidade(vm);
                }

                entidade = _entidadeService.SalvarEntidade(RecuperarUsuarioId(), entidade);

                return JsonFormResponse(entidade.Id);
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

        private Entidade CriarEntidade(SalvarEntidadeViewModel vm)
        {
            TipoDeEntidade tipo = _entidadeService.RecuperarTipoDeEntidade(vm.TipoEntidadeId);
            Municipio municipio = _cadastroService.RecuperarMunicipio(vm.MunicipioId);

            var entidade = new Entidade
            {
                Sigla = vm.Sigla.Trim(),
                Nome = vm.Nome.Trim(),
                Municipio = municipio,
                Tipo = tipo
            };

            var pessoas = _pessoaService.RecuperarPessoas(vm.Gestores);

            foreach (var pessoa in pessoas)
            {
                entidade.AdicionarGestor(pessoa);
            }

            return entidade;
        }

        private Entidade AtualizarEntidade(SalvarEntidadeViewModel vm)
        {
            Entidade entidade = _entidadeService.RecuperarEntidadeCompleta(vm.Id);
            TipoDeEntidade tipo = _entidadeService.RecuperarTipoDeEntidade(vm.TipoEntidadeId);
            Municipio municipio = _cadastroService.RecuperarMunicipio(vm.MunicipioId);

            entidade.Municipio = municipio;
            entidade.Tipo = tipo;
            entidade.Sigla = vm.Sigla.Trim();
            entidade.Nome = vm.Nome.Trim();
            entidade.Gestores = new List<Gestor>();

            var pessoas = _pessoaService.RecuperarPessoas(vm.Gestores);

            foreach (var pessoa in pessoas)
            {
                entidade.AdicionarGestor(pessoa);
            }

            return entidade;
        }

        private void ValidarCadastroEntidade(SalvarEntidadeViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Nome))
            {
                ModelState.AddModelError("Nome", "Preencha o Nome da Entidade.");
            }
            if (string.IsNullOrWhiteSpace(vm.Sigla))
            {
                ModelState.AddModelError("Sigla", "Preencha a Sigla da Entidade.");
            }
            if (vm.TipoEntidadeId <= 0)
            {
                ModelState.AddModelError("Tipo", "Informe o Tipo da Entidade.");
            }
            if (vm.MunicipioId <= 0)
            {
                ModelState.AddModelError("Município", "Informe o Município.");
            }

            if (!ModelState.IsValid)
            {
                throw new Exception("Preencha o formulário corretamente.");
            }
        }
    }
}