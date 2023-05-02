using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Trilhas.Controllers.Mappers;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Data.Model.Exceptions;
using Trilhas.Data.Model.Trilhas;
using Trilhas.Models.Cadastros.Docente;
using Trilhas.Services;

namespace Trilhas.Controllers
{
    public class DocentesController : DefaultController
    {
        private readonly PessoaService _pessoaService;
        private readonly DocenteService _docenteService;
        private readonly SolucaoEducacionalService _solucaoEducacionalService;
        private readonly DocenteMapper _mapper;

        public DocentesController(UserManager<IdentityUser> userManager, DocenteService docenteService, PessoaService pessoaService, SolucaoEducacionalService solucaoEducacionalService, EventoService eventoService) : base(userManager)
        {
            _docenteService = docenteService;
            _pessoaService = pessoaService;
            _solucaoEducacionalService = solucaoEducacionalService;
            _mapper = new DocenteMapper();
        }

        [HttpPost]
        public IActionResult Salvar([FromBody] SalvarDocenteViewModel vm)
        {
            try
            {
                ValidarCadastroDocente(vm);

                Docente docente;

                if (vm.Id > 0)
                {
                    docente = AtualizarDocente(vm);
                }
                else
                {
                    docente = CriarDocente(vm);
                }

                docente = _docenteService.SalvarDocente(RecuperarUsuarioId(), docente);

                return JsonFormResponse(docente.Id);
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

        [HttpGet]
        public IActionResult Buscar(string nome, string email, string cpf, string numeroFuncional, bool excluidos, int start = -1, int count = -1)
        {
            var docentes = _docenteService.PesquisarDocentes(nome, email, cpf, numeroFuncional, excluidos, start, count);

            var vm = _mapper.MapearGridDocente(docentes);

            return Json(vm);
        }

        [HttpGet]
        public IActionResult Quantidade(string nome, string email, string cpf, string numeroFuncional, bool exibirExcluidos)
        {
            var docentes = _docenteService.QuantidadeDeDocentes(nome, email, cpf, numeroFuncional, exibirExcluidos);
            return Json(docentes);
        }

        [HttpGet]
        public IActionResult BuscarDadosGrid(string nomeDocente, long cursoId, int? modalidadeCurso, DateTime? dataInicio, DateTime? dataFim, bool excluidos, int start = 0, int count = -1)
        {
            var docentes = _docenteService.PesquisarDocenteSqlQuery(nomeDocente, cursoId, modalidadeCurso, dataInicio, dataFim, excluidos);

            var vm = _mapper.MapearDadosGridDocente(docentes);

            var x = vm.AsQueryable();

            if (start > 0)
            {
                x = x.Skip(start);
            }
            if (count > 0)
            {
                x = x.Take(count);
            }

            return Json(x.ToList());
        }

        [HttpGet]
        public IActionResult QuantidadeGrid(string nomeDocente, long cursoId, int? modalidadeCurso, DateTime? dataInicio, DateTime? dataFim, bool excluidos)
        {
            var qtd = _docenteService.PesquisarQuantidadeDocenteSqlQuery(nomeDocente, cursoId, modalidadeCurso, dataInicio, dataFim, excluidos);
            return new ObjectResult(qtd);
        }

        [HttpGet]
        public IActionResult Recuperar(long id)
        {
            var docente = _docenteService.RecuperarDocenteCompleto(id, false);

            var vm = _mapper.MapearDocente(docente);

            return Json(vm);
        }

        [HttpGet]
        public IActionResult RecuperarBasico(long id)
        {
            var docente = _docenteService.RecuperarDocente(id, false);

            var vm = _mapper.MapearDocente(docente);

            return Json(vm);
        }

        [HttpDelete]
        public IActionResult Excluir(long id)
        {
            try
            {
                _docenteService.ExcluirDocente(RecuperarUsuarioId(), id);
            }
            catch (RecordNotFoundException x)
            {
                return BadRequest(x.Message);
            }
            catch (Exception x)
            {
                return StatusCode(500, x.Message);
            }

            return new EmptyResult();
        }

        private void ValidarCadastroDocente(SalvarDocenteViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Pis))
            {
                ModelState.AddModelError("PIS/PASEP", "Informe o PIS/PASEP do Docente.");
            }
            if (string.IsNullOrWhiteSpace(vm.Titulo))
            {
                ModelState.AddModelError("Título", "Informe o Número do Titulo do Docente.");
            }
            if (vm.DadosBancarios.Count <= 0)
            {
                ModelState.AddModelError("Dados Bancários", "Informe pelo menos uma Conta Bancária do Docente.");
            }
            if (vm.Formacao.Count <= 0)
            {
                ModelState.AddModelError("Formação", "Informe pelo menos uma Formação do Docente.");
            }

            if (!ModelState.IsValid)
            {
                throw new Exception("Preencha o formulário corretamente.");
            }
        }

        private Docente CriarDocente(SalvarDocenteViewModel vm)
        {
            Pessoa pessoa = _pessoaService.RecuperarPessoa(vm.PessoaId);

            Docente docente = new Docente
            {
                Pessoa = pessoa,
                Observacoes = vm.Observacoes,
                DadosBancarios = new List<DadosBancarios>(),
                Formacao = new List<Formacao>(),
                Habilitacao = new List<Habilitacao>()
            };

            docente.DadosBancarios = SalvarListaDadosBancarios(docente, vm.DadosBancarios);
            docente.Formacao = SalvarListaDocenteFormacao(docente, vm.Formacao);

            if (vm.Habilitacao.Count > 0)
            {
                docente.Habilitacao = SalvarListaDocenteHabilitacao(docente, vm.Habilitacao);
            }

            docente.Pessoa.Pis = vm.Pis;
            docente.Pessoa.NumeroTitulo = vm.Titulo;

            return docente;
        }

        private Docente AtualizarDocente(SalvarDocenteViewModel vm)
        {
            Docente docente = _docenteService.RecuperarDocente(vm.Id);

            docente.Pessoa.Pis = vm.Pis;
            docente.Pessoa.NumeroTitulo = vm.Titulo;
            docente.Observacoes = vm.Observacoes;
            docente.Habilitacao = SalvarListaDocenteHabilitacao(docente, vm.Habilitacao);
            docente.Formacao = SalvarListaDocenteFormacao(docente, vm.Formacao);

            return docente;
        }

        private List<Habilitacao> SalvarListaDocenteHabilitacao(Docente docente, List<DocenteHabilitacaoViewModel> vmHabilitacoes)
        {
            List<Habilitacao> listaHabilitacoes = new List<Habilitacao>();
            Habilitacao habilitacaoAux;

            foreach (var vm in vmHabilitacoes)
            {
                if (vm.Id > 0)
                {
                    habilitacaoAux = _docenteService.RecuperarHabilitacao(vm.Id);
                }
                else
                {
                    habilitacaoAux = new Habilitacao
                    {
                        Docente = docente
                    };
                }

                habilitacaoAux.Curso = (Curso)_solucaoEducacionalService.RecuperarSolucaoEducacional(vm.Curso.Id);
                listaHabilitacoes.Add(habilitacaoAux);
            }

            return listaHabilitacoes;
        }

        private List<Formacao> SalvarListaDocenteFormacao(Docente docente, List<DocenteFormacaoViewModel> formacoes)
        {
            List<Formacao> listaFormacoes = new List<Formacao>();
            Formacao formacaoAux;

            foreach (var formacao in formacoes)
            {
                if (formacao.Id > 0)
                {
                    formacaoAux = _docenteService.RecuperarFormacao(formacao.Id);
                }
                else
                {
                    formacaoAux = new Formacao
                    {
                        Docente = docente
                    };
                }

                formacaoAux.Curso = formacao.Curso;
                formacaoAux.Titulacao = formacao.Titulacao;
                formacaoAux.Instituicao = formacao.Instituicao;
                formacaoAux.CargaHoraria = formacao.CargaHoraria;
                formacaoAux.DataInicio = formacao.DataInicio;
                formacaoAux.DataFim = formacao.DataFim;

                listaFormacoes.Add(formacaoAux);
            }

            return listaFormacoes;
        }

        private List<DadosBancarios> SalvarListaDadosBancarios(Docente docente, List<DocenteDadosBancariosViewModel> dadosBancarios)
        {
            List<DadosBancarios> listaDadosBancarios = new List<DadosBancarios>();

            foreach (var dadosBancario in dadosBancarios)
            {
                DadosBancarios dadosBancariosAux;

                if (dadosBancario.Id > 0)
                {
                    dadosBancariosAux = RecuperarDadosBancarios(dadosBancario.Id) ?? new DadosBancarios();
                }
                else
                {
                    dadosBancariosAux = new DadosBancarios
                    {
                        Docente = docente
                    };
                }

                dadosBancariosAux.Banco = dadosBancario.Banco;
                dadosBancariosAux.ContaCorrente = dadosBancario.ContaCorrente;
                dadosBancariosAux.Agencia = dadosBancario.Agencia;
                listaDadosBancarios.Add(dadosBancariosAux);
            }

            return listaDadosBancarios;
        }

        private DadosBancarios RecuperarDadosBancarios(long idDadosBancarios)
        {
            DadosBancarios dadosBancarios = _docenteService.RecuperarDadosBancarios(idDadosBancarios);
            return dadosBancarios;
        }
    }
}