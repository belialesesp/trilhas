using BundlerMinifier;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trilhas.Controllers.Mappers;
using Trilhas.Data.Enums;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Data.Model.Eventos;
using Trilhas.Data.Model.Exceptions;
using Trilhas.Data.Model.Trilhas;
using Trilhas.Helper;
using Trilhas.Models;
using Trilhas.Models.Evento;
using Trilhas.Services;

namespace Trilhas.Controllers
{
    [Authorize(Roles = "Administrador,Secretaria,Gestor,Coordenador")]
    public class EventosController : DefaultController
    {
        private readonly EventoService _eventoService;
        private readonly RecursoService _recursoService;
        private readonly PessoaService _pessoaService;
        private readonly SolucaoEducacionalService _solucaoService;
        private readonly EntidadeService _entidadeService;
        private readonly LocalService _localService;
        private readonly DocenteService _docenteService;
        private readonly IConfiguration _configuration;
        private readonly EventoMapper _mapper;
        private readonly CertificadoService _certificadoService;
        private readonly RelatorioService _relatorioService;

        public EventosController(UserManager<IdentityUser> userManager,
            EventoService eventoService,
            RecursoService recursoService,
            LocalService localService,
            EntidadeService entidadeService,
            SolucaoEducacionalService solucaoService,
            PessoaService pessoaService,
            DocenteService docenteService,
            IConfiguration configuration,
            CertificadoService certificadoService,
            RelatorioService relatorioService
            ) : base(userManager)
        {
            _eventoService = eventoService;
            _solucaoService = solucaoService;
            _pessoaService = pessoaService;
            _entidadeService = entidadeService;
            _localService = localService;
            _recursoService = recursoService;
            _docenteService = docenteService;
            _certificadoService = certificadoService;
            _configuration = configuration;
            _relatorioService = relatorioService;
            _mapper = new EventoMapper();
        }

        [HttpGet]
        public IActionResult Quantidade(long cursoId,
                                    EnumModalidade? modalidade,
                                    long entidadeDemandanteId,
                                    long municipioId,
                                    long docenteId,
                                    long cursistaId,
                                    DateTime dataInicio,
                                    DateTime dataFim,
                                    bool naoIniciados,
                                    bool andamentos,
                                    bool concluidos,
                                    bool cancelados,
                                    bool inscricao,
                                    string curso,
                                    string entidadeDemandante,
                                    string docente,
                                    bool finalizados)
        {
            if (dataFim == DateTime.MinValue)
            {
                dataFim = DateTime.MaxValue;
            }

            int qtd = _eventoService.QuantidadeDeEventos(cursoId, curso, modalidade, entidadeDemandanteId, entidadeDemandante, municipioId, docenteId, docente, cursistaId, dataInicio, dataFim, naoIniciados, andamentos, concluidos, cancelados, inscricao, finalizados);
            return new ObjectResult(qtd);
        }

        [HttpGet]
        public IActionResult Buscar(long cursoId,
                                    EnumModalidade? modalidade,
                                    long entidadeDemandanteId,
                                    long municipioId,
                                    long docenteId,
                                    long cursistaId,
                                    DateTime dataInicio,
                                    DateTime dataFim,
                                    bool naoIniciados,
                                    bool andamentos,
                                    bool concluidos,
                                    bool cancelados,
                                    bool inscricao,
                                    string curso,
                                    string entidadeDemandante,
                                    bool finalizados,
                                    string docente, int start = -1, int count = -1)
        {
            if (dataFim == DateTime.MinValue)
            {
                dataFim = DateTime.MaxValue;
            }

            List<Evento> eventos = _eventoService.PesquisarEventos(cursoId, curso, modalidade, entidadeDemandanteId, entidadeDemandante, municipioId, docenteId, docente, cursistaId, dataInicio, dataFim, naoIniciados, andamentos, concluidos, cancelados, inscricao, finalizados, start, count);

            var vm = _mapper.MapearEventosViewModel(eventos);

            return Json(vm);
        }

        [HttpGet]
        public IActionResult ExportarRelatorioCapacitadosPorPeriodoExcel(long cursoId,
                                    EnumModalidade? modalidade,
                                    long entidadeDemandanteId,
                                    long municipioId,
                                    long docenteId,
                                    long cursistaId,
                                    DateTime dataInicio,
                                    DateTime dataFim,
                                    bool naoIniciados,
                                    bool andamentos,
                                    bool concluidos,
                                    bool cancelados,
                                    bool inscricao,
                                    string curso,
                                    string entidadeDemandante,
                                    bool finalizados,
                                    string docente, int start = -1, int count = -1)
        {
            if (dataFim == DateTime.MinValue)
            {
                dataFim = DateTime.MaxValue;
            }

            List<Evento> eventos = _eventoService.PesquisarEventos(cursoId, curso, modalidade, entidadeDemandanteId, entidadeDemandante, municipioId, docenteId, docente, cursistaId, dataInicio, dataFim, naoIniciados, andamentos, concluidos, cancelados, inscricao, finalizados, -1, -1);

            var relatorio = _relatorioService.GerarPlanilhaRelatorioCapacitadosPorPeriodo(eventos);
                        
            return new ObjectResult(relatorio);
        }



        [HttpGet]
        public IActionResult ExportarRelatorioCapacitadosPorCursoExcel(long id)
        {
            Evento evento = _eventoService.RecuperarEventoCompleto(id);


            var vm = _mapper.MapearEventoFinalizadoViewModel(evento);


            var relatorio = _relatorioService.GerarPlanilhaRelatorioCapacitadosPorCurso(vm);

            return new ObjectResult(relatorio);
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Secretaria,Gestor")]
        public IActionResult BuscarCursistas(long eventoId, string nome, long entidadeId, string cpf, int start = -1, int count = -1)
        {
            Evento evento = _eventoService.RecuperarEventoInscricao(eventoId);

            if (evento == null)
            {
                return new EmptyResult();
            }

            long gestorId = 0;

            if (UsuarioGestor())
            {
                entidadeId = 2;
                var gestor = _pessoaService.RecuperarPessoaPorCpf("");

                if (gestor != null)
                {
                    gestorId = gestor.Id;
                }
            }

            List<Pessoa> cursistas = _pessoaService.PesquisarPessoas(nome, null, cpf, null, entidadeId, gestorId, false, start, count);

            var vm = _mapper.MapearCursistasViewModel(evento, cursistas);

            return Json(vm);
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Secretaria,Gestor")]
        public IActionResult BuscarCursistasQuantidade(string nome, long entidadeId, string cpf, int start = -1, int count = -1)
        {
            long gestorId = 0;

            if (UsuarioGestor())
            {
                entidadeId = 2;
                var gestor = _pessoaService.RecuperarPessoaPorCpf("");

                if (gestor != null)
                {
                    gestorId = gestor.Id;
                }
            }

            int qtd = _pessoaService.QuantidadeDePessoas(nome, null, cpf, null, entidadeId, gestorId, false);
            return new ObjectResult(qtd);
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Secretaria")]
        public IActionResult Recuperar(long id)
        {
            Evento evento = _eventoService.RecuperarEventoEdicao(id);

            var vm = _mapper.MapearEventoViewModel(evento);

            return Json(vm);
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Secretaria,Coordenador")]
        public IActionResult Encerramento(long id)
        {
            try
            {
                Evento evento = _eventoService.RecuperarEventoCompleto(id);

                var vm = _mapper.MapearEventoFinalizadoViewModel(evento);

                return Json(vm);
            }
            catch (TrilhasException tex)
            {
                return JsonErrorFormResponse(tex);
            }
            catch (Exception ex)
            {
                return JsonErrorFormResponse(ex, "Ocorreu um erro ao finalizar o Evento.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Secretaria,Gestor,Coordenador")]
        public IActionResult FinalizarEvento(long eventoId)
        {
            try
            {
                Evento evento = _eventoService.RecuperarEventoCompleto(eventoId);

                if (evento != null)
                {
                    int prazoPenalidade = _configuration.GetValue<int>("Parametros:PrazoPenalidade");

                    _eventoService.FinalizarEvento(RecuperarUsuarioId(), evento, prazoPenalidade);
                }

                return JsonFormResponse(eventoId);
            }
            catch (TrilhasException tex)
            {
                return JsonErrorFormResponse(tex);
            }
            catch (Exception ex)
            {
                return JsonErrorFormResponse(ex, "Ocorreu um erro ao finalizar o Evento.");
            }
        }


        [HttpPost]
        [Authorize(Roles = "Administrador,Secretaria,Gestor,Coordenador")]
        public IActionResult SalvarPenalidade([FromBody] PenalidadeViewModel penalidade)
        {
            try
            {
                _eventoService.SalvarPenalidade(new Penalidade
                {
                    Id = penalidade.Id,
                    JustificativaDeCancelamento = penalidade.Justificativa,
                    DataDaPenalidade = penalidade.DataDaPenalidade,
                    DataFimPenalidade = penalidade.DataFim,
                    DataInicioPenalidade = penalidade.DataInicio
                });

                return JsonFormResponse(penalidade.Id);
            }
            catch (Exception ex)
            {
                return JsonErrorFormResponse(ex, "Ocorreu um erro ao salvar penalidade.");
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Administrador,Secretaria")]
        public IActionResult Excluir(long id, string motivoExclusao)
        {
            try
            {
                if (string.IsNullOrEmpty(motivoExclusao))
                {
                    throw new Exception("Informe o motivo da exclusão");
                }

                _eventoService.ExcluirEvento(RecuperarUsuarioId(), id, motivoExclusao);
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
        [Authorize(Roles = "Administrador,Secretaria")]
        public IActionResult Salvar([FromBody] SalvarEventoViewModel vm)
        {
            try
            {
                ValidarCadastroEvento(vm);

                Evento evento;

                if (vm.Id > 0)
                {
                    evento = AtualizarEvento(vm);
                }
                else
                {
                    evento = CriarEvento(vm);
                }

                _eventoService.SalvarEvento(RecuperarUsuarioId(), evento);

                return JsonFormResponse(evento.Id);
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
        [Authorize(Roles = "Administrador,Secretaria,Gestor")]
        public IActionResult RecuperarListaInscritos(long id)
        {
            var lista = _eventoService.RecuperarListaInscricao(id);

            var vm = _mapper.MapearListaInscritosViewModel(lista);

            return Json(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Secretaria,Gestor")]
        public IActionResult InscreverCursista([FromBody] InscreverViewModel vm)
        {
            try
            {
                ValidarDadosInscricao(vm);

                Evento evento = _eventoService.RecuperarEventoInscricao(vm.EventoId);
                Pessoa cursista = _pessoaService.RecuperarPessoaCompleto(vm.PessoaId);

                ValidarPeriodoDeInscricao(evento);

                _eventoService.InscreverCursista(RecuperarUsuarioId(), evento, cursista, vm.DataDeInscricao);

                return Json(cursista.NomeSocial ?? cursista.Nome);
            }
            catch (TrilhasException tex)
            {
                return JsonErrorFormResponse(tex);
            }
            catch (Exception ex)
            {
                return JsonErrorFormResponse(ex, "Ocorreu um erro na Inscrição do Cursista.");
            }
        }

        private void ValidarPeriodoDeInscricao(Evento evento)
        {
            var hoje = DateTime.Now;

            var agenda = evento.Agenda();

            if (!User.IsInRole("Administrador") && !(hoje >= agenda.DataHoraInscricaoInicio && hoje <= agenda.DataHoraInscricaoFim))
            {
                throw new TrilhasException("O Evento está fora do período de Inscrição.");
            }
        }

        [HttpPost]
        public IActionResult CancelarInscricao([FromBody] CancelarInscricaoViewModel vm)
        {
            try
            {
                Evento evento = _eventoService.RecuperarEventoInscricao(vm.EventoId);

                if (evento != null)
                {
                    Pessoa pessoa = _pessoaService.RecuperarPessoa(vm.Pessoaid);
                    _eventoService.CancelarInscricao(RecuperarUsuarioId(), evento, pessoa);
                }

                return Json("OK");
            }
            catch (TrilhasException tex)
            {
                return JsonErrorFormResponse(tex);
            }
            catch (Exception ex)
            {
                return JsonErrorFormResponse(ex, "Ocorreu um erro ao cancelar a Inscrição do Cursista.");
            }
        }

        [HttpGet]
        public FileContentResult DownloadListaInscritosEvento(long id)
        {
            var evento = _eventoService.RecuperarEventoInscricao(id);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("CPF;NOME;ENTIDADE;EMAIL");

            foreach (var inscrito in evento.ListaDeInscricao.PessoasInscritas())
            {
                sb.Append(inscrito.Cpf);
                sb.Append(";");
                sb.Append(inscrito.NomeSocial ?? inscrito.Nome);
                sb.Append(";");
                sb.Append(evento.EntidadeDemandante.Nome);
                sb.Append(";");
                sb.Append(inscrito.Email);
                sb.AppendLine();
            }

            return File(Encoding.GetEncoding("iso-8859-1").GetBytes(sb.ToString()), "text/csv", "ListaInscritos.csv");
        }

        [HttpGet]
        public IActionResult RecuperarFuncoesDocente()
        {
            List<DropDownViewModel> funcoes = _mapper.MapearDropDownViewModel(_eventoService.RecuperarFuncoesDocente());
            return Json(funcoes);
        }

        private Evento CriarEvento(SalvarEventoViewModel vm)
        {
            Pessoa coordenador = null;

            if (vm.Coordenador != null && vm.Coordenador.Id > 0)
                coordenador = _pessoaService.RecuperarPessoa(vm.Coordenador.Id);

            Curso curso = (Curso)_solucaoService.RecuperarSolucaoEducacionalCompleta(vm.Curso.Id);
            Entidade entidade = _entidadeService.RecuperarEntidade(vm.Entidade.Id);

            Evento evento = new Evento
            {
                Coordenador = coordenador,
                Curso = curso,
                EntidadeDemandante = entidade,
                Observacoes = vm.Observacoes,
                LimitarInscricoes = vm.LimitarVagas,
                VagasPorEntidade = vm.VagasPorEntidade
            };

            if (curso.Modalidade == EnumModalidade.EAD)
            {
                evento.UrlEad = vm.UrlEad;
            }
            else
            {
                Local local = _localService.RecuperarLocalCompleto(vm.Local.Id);
                evento.Local = local;
            }

            if (vm.CertificadoId.HasValue)
            {
                evento.Certificado = _certificadoService.RecuperarCertificado(vm.CertificadoId.Value, null);
            }

            evento.Horarios = vm.Horarios != null ? CriarListaEventoHorario(evento, vm.Horarios) : null;
            evento.Recursos = vm.Recursos != null ? CriarListaEventoRecurso(evento, vm.Recursos) : null;
            evento.Cotas = vm.Cotas != null ? CriarListaEventoCota(evento, vm.Cotas) : null;
            evento.DeclaracaoCursista = _certificadoService.RecuperarCertificado(vm.DeclaracaoCursistaId, null);
            evento.DeclaracaoDocente = _certificadoService.RecuperarCertificado(vm.DeclaracaoDocenteId, null);

            if (vm.Agenda != null)
            {
                if (evento.Agendas == null)
                {
                    evento.Agendas = new List<EventoAgenda>();
                }

                evento.Agendas.Add(CriarListaEventoAgenda(evento, vm.Agenda));
            }

            return evento;
        }

        private Evento AtualizarEvento(SalvarEventoViewModel vm)
        {
            Pessoa coordenador = null;
            
            if(vm.Coordenador != null && vm.Coordenador.Id > 0)
                coordenador = _pessoaService.RecuperarPessoa(vm.Coordenador.Id);

            Evento evento = _eventoService.RecuperarEventoCompleto(vm.Id);
            Curso curso = (Curso)_solucaoService.RecuperarSolucaoEducacionalCompleta(vm.Curso.Id);
            Entidade entidade = _entidadeService.RecuperarEntidade(vm.Entidade.Id);

            if (curso.Modalidade != EnumModalidade.EAD)
            {
                evento.Local = _localService.RecuperarLocalCompleto(vm.Local.Id);
            }
            else
            {
                evento.Local = null;
            }

            if (vm.CertificadoId.HasValue)
            {
                evento.Certificado = _certificadoService.RecuperarCertificado(vm.CertificadoId.Value, null);
            }

            evento.Coordenador = coordenador;
            evento.Curso = curso;
            evento.EntidadeDemandante = entidade;
            evento.Observacoes = vm.Observacoes;
            evento.UrlEad = vm.UrlEad;
            evento.LimitarInscricoes = vm.LimitarVagas;
            evento.VagasPorEntidade = vm.VagasPorEntidade;

            evento.Horarios = vm.Horarios != null ? CriarListaEventoHorario(evento, vm.Horarios) : null;
            evento.Recursos = vm.Recursos != null ? CriarListaEventoRecurso(evento, vm.Recursos) : null;
            evento.Cotas = vm.Cotas != null ? CriarListaEventoCota(evento, vm.Cotas) : null;
            evento.DeclaracaoCursista = _certificadoService.RecuperarCertificado(vm.DeclaracaoCursistaId, null);
            evento.DeclaracaoDocente = _certificadoService.RecuperarCertificado(vm.DeclaracaoDocenteId, null);

            if (vm.Agenda != null)
            {
                if (evento.Agendas == null)
                {
                    evento.Agendas = new List<EventoAgenda>();
                }

                evento.Agendas.Add(CriarListaEventoAgenda(evento, vm.Agenda));
            }

            return evento;
        }

        private void ValidarCadastroEvento(SalvarEventoViewModel vm)
        {
            if (vm.Curso.Id <= 0)
            {
                ModelState.AddModelError("Solução Educacional", "Solução Educacional não Informada.");
            }
            if (vm.Entidade.Id <= 0)
            {
                ModelState.AddModelError("Entidade Demandante", "Entidade Demandante não Informada.");
            }
            if (vm.Curso.ModalidadeDeCurso != EnumModalidade.EAD)
            {
                if (vm.Local.Id <= 0)
                {
                    ModelState.AddModelError("Local", "Local não Informado.");
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(vm.UrlEad))
                {
                    ModelState.AddModelError("Local", "URL não Informada.");
                }
            }

            if (vm.Agenda.DataInscricaoInicio.Equals(DateTime.MinValue))
            {
                ModelState.AddModelError("Agenda", "A Data do Início das inscrições não foi informada.");
            }
            if (vm.Agenda.DataInscricaoFim.Equals(DateTime.MinValue))
            {
                ModelState.AddModelError("Agenda", "A Data do Fim das inscrições não foi informada.");
            }
            if (vm.Agenda.HoraInscricaoInicio.Equals(DateTime.MinValue))
            {
                ModelState.AddModelError("Agenda", "O Horário do Início das inscrições não foi informado.");
            }
            if (vm.Agenda.HoraInscricaoFim.Equals(DateTime.MinValue))
            {
                ModelState.AddModelError("Agenda", "O Horário do Fim das inscrições não foi informado.");
            }

            if (vm.Horarios.Count <= 0)
            {
                ModelState.AddModelError("Horário", "Nenhum horário informado.");
            }
            else
            {
                if (vm.Horarios.Any(x => x.ModuloId <= 0))
                {
                    ModelState.AddModelError("Horário", "Existe(m) horário(m) sem Módulo informado.");
                }
                if (vm.Curso.ModalidadeDeCurso != EnumModalidade.EAD && vm.Horarios.Any(x => x.SalaId <= 0 && x.DataInicio > DateTime.Now))
                {
                    ModelState.AddModelError("Horário", "Existe(m) horário(s) sem Sala informada.");
                }
                if (vm.Horarios.Any(x => x.DocenteId <= 0))
                {
                    ModelState.AddModelError("Horário", "Existe(m) horário(s) sem Docente informado.");
                }
                if (vm.Horarios.Any(x => x.FuncaoDocenteId <= 0))
                {
                    ModelState.AddModelError("Horário", "Existe(m) horário(s) sem Função de Docente informado.");
                }
            }

            if (vm.LimitarVagas && vm.VagasPorEntidade <= 0)
            {
                ModelState.AddModelError("Limite de Vagas", "Informe a quantidade de vagas por Entidade.");
            }

            if (!ModelState.IsValid)
            {
                throw new Exception("Preencha o formulário corretamente.");
            }
        }

        private List<EventoCota> CriarListaEventoCota(Evento evento, List<SalvarEventoCotaViewModel> vmCotas)
        {
            List<EventoCota> listaCotas = new List<EventoCota>();

            foreach (var cota in vmCotas)
            {
                EventoCota eventoCota;

                if (cota.Id > 0)
                {
                    eventoCota = evento.Cotas.FirstOrDefault(x => x.Id == cota.Id);
                    //_eventoService.RecuperarEventoCota(cota.Id);
                }
                else
                {
                    eventoCota = new EventoCota
                    {
                        Evento = evento
                    };
                }

                eventoCota.Entidade = _entidadeService.RecuperarEntidade(cota.EntidadeId);
                eventoCota.Quantidade = cota.Quantidade;
                listaCotas.Add(eventoCota);
            }

            return listaCotas;
        }

        private EventoAgenda CriarListaEventoAgenda(Evento evento, SalvarEventoAgendaViewModel vmAgenda)
        {
            DateTime dataHoraInicioInscricao;
            DateTime dataHoraFimInscricao;
            EventoAgenda eventoAgenda;

            if (vmAgenda.Id > 0)
            {
                eventoAgenda = evento.Agendas.FirstOrDefault(x => x.Id == vmAgenda.Id);
            }
            else
            {
                eventoAgenda = new EventoAgenda
                {
                    Evento = evento
                };
            }

            vmAgenda.DataInscricaoInicio = (vmAgenda.DataInscricaoInicio.Kind == DateTimeKind.Utc) ? vmAgenda.DataInscricaoInicio.ToLocalTime() : vmAgenda.DataInscricaoInicio;
            vmAgenda.DataInscricaoFim = (vmAgenda.DataInscricaoFim.Kind == DateTimeKind.Utc) ? vmAgenda.DataInscricaoFim.ToLocalTime() : vmAgenda.DataInscricaoFim;
            vmAgenda.HoraInscricaoInicio = (vmAgenda.HoraInscricaoInicio.Kind == DateTimeKind.Utc) ? vmAgenda.HoraInscricaoInicio.ToLocalTime() : vmAgenda.HoraInscricaoInicio;
            vmAgenda.HoraInscricaoFim = (vmAgenda.HoraInscricaoFim.Kind == DateTimeKind.Utc) ? vmAgenda.HoraInscricaoFim.ToLocalTime() : vmAgenda.HoraInscricaoFim;

            dataHoraInicioInscricao = vmAgenda.DataInscricaoInicio.Date;
            eventoAgenda.DataHoraInscricaoInicio = dataHoraInicioInscricao.Add(vmAgenda.HoraInscricaoInicio.TimeOfDay);

            dataHoraFimInscricao = vmAgenda.DataInscricaoFim.Date;
            eventoAgenda.DataHoraInscricaoFim = dataHoraFimInscricao.Add(vmAgenda.HoraInscricaoFim.TimeOfDay);

            eventoAgenda.DataHoraInicio = evento.Horarios.Min(x => x.DataHoraInicio);
            eventoAgenda.DataHoraFim = evento.Horarios.Max(x => x.DataHoraFim);

            eventoAgenda.NumeroVagas = vmAgenda.NumeroVagas;
            eventoAgenda.Justificativa = vmAgenda.Justificativa;

            return eventoAgenda;
        }

        private List<EventoHorario> CriarListaEventoHorario(Evento evento, List<SalvarEventoHorarioViewModel> vmHorarios)
        {
            List<EventoHorario> listaHorarios = new List<EventoHorario>();
            DateTime dataHoraInicio;
            DateTime dataHoraFim;

            foreach (var horario in vmHorarios)
            {
                Docente docente = _docenteService.RecuperarDocente(horario.DocenteId);

                EventoHorario eventoHorario;

                if (horario.Id > 0)
                {
                    eventoHorario = evento.Horarios.FirstOrDefault(x => x.Id == horario.Id);
                }
                else
                {
                    eventoHorario = new EventoHorario
                    {
                        Evento = evento
                    };
                }

                eventoHorario.Docente = docente;
                eventoHorario.Funcao = _eventoService.RecuperarFuncaoDocente(horario.FuncaoDocenteId);
                eventoHorario.Modulo = _solucaoService.RecuperarModulo(horario.ModuloId);

                horario.DataInicio = (horario.DataInicio.Kind == DateTimeKind.Utc) ? horario.DataInicio.ToLocalTime() : horario.DataInicio;
                horario.HoraInicio = (horario.HoraInicio.Kind == DateTimeKind.Utc) ? horario.HoraInicio.ToLocalTime() : horario.HoraInicio;
                horario.DataFim = (horario.DataFim.Kind == DateTimeKind.Utc) ? horario.DataFim.ToLocalTime() : horario.DataFim;
                horario.HoraFim = (horario.HoraFim.Kind == DateTimeKind.Utc) ? horario.HoraFim.ToLocalTime() : horario.HoraFim;

                dataHoraInicio = horario.DataInicio.Date;
                eventoHorario.DataHoraInicio = dataHoraInicio.Add(horario.HoraInicio.TimeOfDay);
                dataHoraFim = horario.DataFim.Date;
                eventoHorario.DataHoraFim = dataHoraFim.Add(horario.HoraFim.TimeOfDay);

                //eventoHorario.DataHoraInicio = (dataHoraInicio.Kind == DateTimeKind.Utc) ? dataHoraInicio.ToLocalTime() : dataHoraInicio;
                //eventoHorario.DataHoraFim = (dataHoraFim.Kind == DateTimeKind.Utc) ? dataHoraFim.ToLocalTime() : dataHoraFim;

                if (evento.Curso.Modalidade != EnumModalidade.EAD)
                {
                    eventoHorario.Sala = _localService.RecuperarLocalSala(horario.SalaId);
                }

                listaHorarios.Add(eventoHorario);
            }

            return listaHorarios;
        }

        private List<EventoRecurso> CriarListaEventoRecurso(Evento evento, List<SalvarEventoRecursoViewModel> vmRecursos)
        {
            List<EventoRecurso> listaRecursos = new List<EventoRecurso>();
            EventoRecurso eventoRecurso;

            foreach (var eventoRecursoVm in vmRecursos)
            {
                if (eventoRecursoVm.Id > 0)
                {
                    eventoRecurso = evento.Recursos.FirstOrDefault(x => x.Id == eventoRecursoVm.Id);
                    //_eventoService.RecuperarEventoRecurso(eventoRecursoVm.Id);
                }
                else
                {
                    eventoRecurso = new EventoRecurso
                    {
                        Evento = evento
                    };
                }

                //vereficar se estava carregando o id dos recursos
                eventoRecurso.Recurso = _recursoService.RecuperarRecurso(eventoRecursoVm.RecursoId);
                eventoRecurso.Quantidade = eventoRecursoVm.Quantidade;
                listaRecursos.Add(eventoRecurso);
            }

            return listaRecursos;
        }

        private void ValidarDadosInscricao(InscreverViewModel vm)
        {
            if (vm.EventoId <= 0)
            {
                ModelState.AddModelError("Evento", "Evento inválido.");
            }
            if (vm.PessoaId <= 0)
            {
                ModelState.AddModelError("Cursista", "Cursista inválido.");
            }

            if (!ModelState.IsValid)
            {
                throw new Exception("Dados para inscrição inválidos.");
            }
        }
    }
}