using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Trilhas.Controllers.Mappers;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Data.Model.Exceptions;
using Trilhas.Models.Cadastros.Local;
using Trilhas.Services;

namespace Trilhas.Controllers
{
    public class LocaisController : DefaultController
    {
        private readonly CadastroService _cadastroService;
        private readonly RecursoService _recursoService;
        private readonly LocalService _service;
        private readonly LocalMapper _mapper;

        public LocaisController(UserManager<IdentityUser> userManager, CadastroService cadastroService, RecursoService recursoService, LocalService localService) : base(userManager)
        {
            _service = localService;
            _recursoService = recursoService;
            _cadastroService = cadastroService;
            _mapper = new LocalMapper();
        }

        [HttpGet]
        public IActionResult Quantidade(string nome, int capacidade, string endereco, bool excluidos)
        {
            int qtd = _service.QuantidadeDeLocais(nome, capacidade, endereco, excluidos);
            return new ObjectResult(qtd);
        }

        [HttpGet]
        public IActionResult Buscar(string nome, int capacidade, string endereco, bool excluidos, int start = -1, int count = -1)
        {
            List<Local> locais = _service.PesquisarLocais(nome, capacidade, endereco, excluidos, start, count);

            var vm = _mapper.MapearLocaisViewModel(locais);

            return Json(vm);
        }

        [HttpGet]
        public IActionResult Recuperar(long id)
        {
            Local local = _service.RecuperarLocalCompleto(id, true);

            var vm = _mapper.MapearLocalViewModel(local, SalasAlocadas(local));

            return Json(vm);
        }

        [HttpGet]
        public IActionResult RecuperarSalas(long id)
        {
            List<LocalSala> salas = _service.RecuperarSalas(id);

            var salasVm = _mapper.MapearLocalSalasViewModel(salas);

            return Json(salasVm);
        }

        [HttpGet]
        public List<TipoLocalContato> RecuperarTiposDeContato()
        {
            return _service.RecuperarTiposLocalContato();
        }

        [HttpDelete]
        public IActionResult Excluir(long id)
        {
            try
            {
                _service.ExcluirLocal(RecuperarUsuarioId(), id);
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
        public IActionResult Salvar([FromBody] SalvarLocalViewModel vm)
        {
            try
            {
                ValidarCadastroLocal(vm);

                Local local;

                if (vm.Id > 0)
                {
                    local = AtualizarLocal(vm);
                }
                else
                {
                    local = CriarLocal(vm);
                }

                var salas = CriarListaSalas(local, vm.Salas);
                var recursos = CriarListaRecurso(local, vm.Recursos);
                var contatos = CriarListaLocalContato(local, vm.Contatos);

                local = _service.SalvarLocal(RecuperarUsuarioId(), local, salas, recursos, contatos);

                return JsonFormResponse(local.Id);
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

        private Local CriarLocal(SalvarLocalViewModel vm)
        {
            var local = new Local
            {
                Nome = string.IsNullOrWhiteSpace(vm.Nome) ? string.Empty : vm.Nome.Trim(),
                Observacoes = string.IsNullOrWhiteSpace(vm.Observacoes) ? string.Empty : vm.Observacoes.Trim(),
                Logradouro = string.IsNullOrWhiteSpace(vm.Logradouro) ? string.Empty : vm.Logradouro.Trim(),
                Bairro = string.IsNullOrWhiteSpace(vm.Bairro) ? string.Empty : vm.Bairro.Trim(),
                Numero = string.IsNullOrWhiteSpace(vm.Numero) ? string.Empty : vm.Numero.Trim(),
                Complemento = string.IsNullOrWhiteSpace(vm.Complemento) ? string.Empty : vm.Complemento.Trim(),
                Cep = vm.Cep,
                Municipio = _cadastroService.RecuperarMunicipio(vm.MunicipioId)
            };

            return local;
        }

        private Local AtualizarLocal(SalvarLocalViewModel vm)
        {
            Local local = _service.RecuperarLocal(vm.Id);

            local.Nome = string.IsNullOrEmpty(vm.Nome) ? string.Empty : vm.Nome.Trim();
            local.Observacoes = string.IsNullOrEmpty(vm.Observacoes) ? string.Empty : vm.Observacoes.Trim();
            local.Logradouro = string.IsNullOrEmpty(vm.Logradouro) ? string.Empty : vm.Logradouro.Trim();
            local.Bairro = string.IsNullOrEmpty(vm.Bairro) ? string.Empty : vm.Bairro.Trim();
            local.Numero = string.IsNullOrEmpty(vm.Numero) ? string.Empty : vm.Numero.Trim();
            local.Complemento = string.IsNullOrEmpty(vm.Complemento) ? string.Empty : vm.Complemento.Trim();
            local.Municipio = _cadastroService.RecuperarMunicipio(vm.MunicipioId);
            local.Cep = vm.Cep;

            return local;
        }

        private void ValidarCadastroLocal(SalvarLocalViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Nome))
            {
                ModelState.AddModelError("Nome", "Preencha o nome do Local.");
            }
            if (vm.Salas.Count <= 0)
            {
                ModelState.AddModelError("Salas", "Informe no mínimo uma sala para o Local.");
            }

            //ENDERECO
            if (string.IsNullOrWhiteSpace(vm.Logradouro))
            {
                ModelState.AddModelError("Logradouro", "Informe logradouro do Local.");
            }
            if (string.IsNullOrWhiteSpace(vm.Bairro))
            {
                ModelState.AddModelError("Bairro", "Informe bairro do Local.");
            }
            if (string.IsNullOrWhiteSpace(vm.Cep))
            {
                ModelState.AddModelError("CEP", "Informe um CEP válido para o Local.");
            }
            if (vm.MunicipioId <= 0)
            {
                ModelState.AddModelError("Cidade", "Informe uma cidade para o Local.");
            }

            if (!ModelState.IsValid)
            {
                throw new Exception("Preencha o formulário corretamente.");
            }
        }

        private List<LocalContato> CriarListaLocalContato(Local local, List<SalvarLocalContatoViewModel> vmContatos)
        {
            var contatos = new List<LocalContato>();

            foreach (var vm in vmContatos)
            {
                contatos.Add(new LocalContato()
                {
                    Id = vm.Id.HasValue ? vm.Id.Value : 0,
                    Local = local,
                    NumeroTelefone = vm.Numero,
                    TipoContato = _service.RecuperarTipoLocalContato(vm.TipoContatoId)
                });
            }

            return contatos;
        }

        private List<LocalRecurso> CriarListaRecurso(Local local, List<SalvarLocalRecursoViewModel> vmRecursos)
        {
            var recursos = new List<LocalRecurso>();

            foreach (var vm in vmRecursos)
            {
                recursos.Add(new LocalRecurso()
                {
                    Id = vm.Id.HasValue ? vm.Id.Value : 0,
                    Local = local,
                    Recurso = _recursoService.RecuperarRecurso(vm.RecursoId),
                    Quantidade = vm.Quantidade
                });
            }

            return recursos;
        }

        private List<LocalSala> CriarListaSalas(Local local, List<SalvarLocalSalaViewModel> vmSalas)
        {
            var salas = new List<LocalSala>();

            if (vmSalas != null)
            {
                foreach (var vm in vmSalas)
                {
                    salas.Add(new LocalSala()
                    {
                        Id = vm.Id.HasValue ? vm.Id.Value : 0,
                        Local = local,
                        CreatorUserId = this.RecuperarUsuarioId(),
                        CreationTime = DateTime.Now,
                        Sigla = vm.Sigla,
                        Numero = vm.Numero,
                        Capacidade = vm.Capacidade,
                    });
                }
            }

            return salas;
        }

        private List<LocalSala> SalasAlocadas(Local local)
        {
            List<LocalSala> alocadas = new List<LocalSala>();

            foreach (var sala in local.Salas)
            {
                if (_service.VerificarSalaAlocada(sala))
                {
                    alocadas.Add(sala);
                }
            }

            return alocadas;
        }
    }
}