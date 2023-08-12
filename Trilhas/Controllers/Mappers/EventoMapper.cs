using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Trilhas.Data.Enums;
using Trilhas.Data.Model;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Data.Model.Eventos;
using Trilhas.Data.Model.Trilhas;
using Trilhas.Extensions;
using Trilhas.Models;
using Trilhas.Models.Cadastros.Docente;
using Trilhas.Models.Evento;
using Trilhas.Models.Evento.ListaPresenca;
using Trilhas.Models.Trilhas.SolucaoEducacional;

namespace Trilhas.Controllers.Mappers
{
    public class EventoMapper
    {
        public EventoViewModel MapearEventoViewModel(Evento evento)
        {
            EventoViewModel eventoVm = new EventoViewModel(evento.Id);

            if (evento.Curso.Modalidade != EnumModalidade.EAD)
            {
                eventoVm.Local = MapearLocalViewModel(evento.Local);
            }
            else
            {
                eventoVm.FlagEad = true;
            }

            eventoVm.Coordenador = MapearCoordenadorViewModel(evento.Coordenador);
            eventoVm.Entidade = MapearEntidadeViewModel(evento.EntidadeDemandante);
            eventoVm.Agenda = MapearEventoAgendaViewModel(evento.Agendas.Last());
            eventoVm.Recursos = MapearEventoRecursoViewModel(evento.Recursos);
            eventoVm.Horarios = MapearEventoHorarioViewModel(evento.Horarios.OrderBy(x => x.DataHoraInicio).ToList());
            eventoVm.Cotas = MapearEventoCotaViewModel(evento.Cotas);
            eventoVm.Curso = MapearEventoSolucaoViewModel(evento.Curso);

            eventoVm.Observacoes = evento.Observacoes;
            eventoVm.Situacao = evento.Situacao().ToString();
            eventoVm.UrlEad = evento.UrlEad;
            eventoVm.LimitarVagas = evento.LimitarInscricoes;
            eventoVm.VagasPorEntidade = evento.VagasPorEntidade;
            eventoVm.ListaDeInscricaoId = evento.ListaDeInscricao.Id;

            if (evento.Curso.PermiteCertificado && evento.Certificado != null)
            {
                eventoVm.CertificadoId = evento.Certificado.Id;
            }
            if (evento.DeclaracaoCursista != null)
            {
                eventoVm.DeclaracaoCursistaId = evento.DeclaracaoCursista.Id;
            }
            if (evento.DeclaracaoDocente != null)
            {
                eventoVm.DeclaracaoDocenteId = evento.DeclaracaoDocente.Id;
            }

            return eventoVm;
        }

        public ModuloViewModel MapearModuloViewModel(Modulo modulo)
        {
            ModuloViewModel moduloVm = new ModuloViewModel
            {
                Id = modulo.Id,
                Nome = modulo.Nome,
                Descricao = modulo.Descricao,
                CargaHoraria = modulo.CargaHoraria
            };

            return moduloVm;
        }

        public EventoLocalViewModel MapearLocalViewModel(Local local)
        {
            EventoLocalViewModel vm = new EventoLocalViewModel(local.Id);
            vm.Nome = local.Nome;
            vm.CapacidadeTotal = local.CapacidadeTotal();

            return vm;
        }

        public EventoSalaViewModel MapearSalaViewModel(LocalSala sala)
        {
            EventoSalaViewModel vm = new EventoSalaViewModel(sala.Id);
            vm.Sigla = sala.Sigla;
            vm.Numero = sala.Numero;
            vm.Capacidade = sala.Capacidade;
            return vm;
        }

        public EventoEntidadeViewModel MapearEntidadeViewModel(Entidade entidade)
        {
            EventoEntidadeViewModel vm = new EventoEntidadeViewModel(entidade.Id);
            vm.Nome = entidade.Sigla + " - " + entidade.Nome;

            return vm;
        }

        public EventoCoordenadorViewModel MapearCoordenadorViewModel(Pessoa coordenador)
        {
            if(coordenador is null)
                return null;

            EventoCoordenadorViewModel vm = new EventoCoordenadorViewModel(coordenador.Id);
            vm.Nome = coordenador.NomeSocial ?? coordenador.Nome;

            return vm;
        }

        public EventoCursoViewModel MapearEventoSolucaoViewModel(Curso solucao)
        {
            EventoCursoViewModel vm = new EventoCursoViewModel(solucao.Id)
            {
                Titulo = solucao.Titulo,
                ModalidadeDeCurso = solucao.Modalidade,
                Modulos = new List<ModuloViewModel>(),
                PermiteCertificado = solucao.PermiteCertificado,
                CargaHorariaTotal = solucao.CargaHorariaTotal()
            };

            foreach (var modulo in solucao.Modulos)
            {
                vm.Modulos.Add(MapearModuloViewModel(modulo));
            }

            return vm;
        }

        public GridEventoCompletaViewModel MapearEventosViewModel(List<Evento> eventos)
        {
            var vm = new GridEventoCompletaViewModel();

            var ch = 0;

            foreach (var evento in eventos)
            {
                SituacaoEvento situacao = evento.Situacao();
                string situacaoDisplay = ((System.ComponentModel.DataAnnotations.DisplayAttribute)situacao.GetType().GetMember(situacao.ToString()).First().GetCustomAttributes(false)[0]).Name;

                vm.ListaEventos.Add(new GridEventoViewModel
                {
                    Id = evento.Id,
                    Entidade = evento.EntidadeDemandante.Sigla,
                    Evento = evento.Curso.Sigla + " - " + evento.Curso.Titulo,
                    CargaHoraria = evento.Curso.CargaHorariaTotal().ToString(),
                    DataInicio = evento.Agendas.LastOrDefault()?.DataHoraInicio,
                    DataFim = evento.Agendas.LastOrDefault()?.DataHoraFim,
                    Municipio = evento.Curso.Modalidade == EnumModalidade.EAD ? "EAD" : evento.Local.Municipio.NomeMunicipio + "-" + evento.Local.Municipio.Uf,
                    Docente = evento.Coordenador != null ? evento.Coordenador.NomeSocial ?? evento.Coordenador.Nome : string.Empty,
                    ListaDeInscricaoId = evento.ListaDeInscricao != null ? evento.ListaDeInscricao.Id : 0,
                    Inscritos = evento.ListaDeInscricao != null ? evento.ListaDeInscricao.Inscritos.Where(x => !x.DeletionTime.HasValue).Count() : 0,
                    Aprovados = evento.ListaDeInscricao != null ? evento.ListaDeInscricao.Inscritos.Where(x => !x.DeletionTime.HasValue && x.Situacao == EnumSituacaoCursista.CERTIFICADO).Count() : 0,
                    Declarados = evento.ListaDeInscricao != null ? evento.ListaDeInscricao.Inscritos.Where(x => !x.DeletionTime.HasValue && x.Situacao == EnumSituacaoCursista.DECLARADO).Count() : 0,
                    Desistentes = evento.ListaDeInscricao != null ? evento.ListaDeInscricao.Inscritos.Where(x => !x.DeletionTime.HasValue && x.Situacao == EnumSituacaoCursista.DESISTENTE).Count() : 0,
                    Situacao = situacaoDisplay,
                    Ead = evento.Curso.Modalidade == EnumModalidade.EAD,
                    Modalidade = evento.Curso.Modalidade.GetDescription()
                });

                ch += evento.Curso.CargaHorariaTotal();
            }

            vm.TotalCargaHoraria = ch;
            vm.TotalInscritos = vm.ListaEventos.Sum(x => x.Inscritos);
            vm.TotalAprovados = vm.ListaEventos.Sum(x => x.Aprovados);
            vm.TotalDeclarados = vm.ListaEventos.Sum(x => x.Declarados);
            vm.TotalDesistentes = vm.ListaEventos.Sum(x => x.Desistentes);

            return vm;
        }

        public EventoAgendaViewModel MapearEventoAgendaViewModel(EventoAgenda agenda)
        {
            EventoAgendaViewModel agendaVm = new EventoAgendaViewModel(agenda.Id);
            agendaVm.DataInicio = agenda.DataHoraInicio;
            agendaVm.DataFim = agenda.DataHoraFim;
            agendaVm.DataInscricaoFim = agenda.DataHoraInscricaoFim;
            agendaVm.DataInscricaoInicio = agenda.DataHoraInscricaoInicio;
            agendaVm.Justificativa = agenda.Justificativa;
            agendaVm.NumeroVagas = agenda.NumeroVagas;

            return agendaVm;
        }

        public List<EventoCotaViewModel> MapearEventoCotaViewModel(List<EventoCota> cotas)
        {
            List<EventoCotaViewModel> cotasVm = new List<EventoCotaViewModel>();
            EventoCotaViewModel vm = null;
            foreach (var cota in cotas)
            {
                vm = new EventoCotaViewModel(cota.Id)
                {
                    EntidadeId = cota.Entidade.Id,
                    EntidadeNome = cota.Entidade.Sigla + " - " + cota.Entidade.Nome,
                    Quantidade = cota.Quantidade
                };
                cotasVm.Add(vm);
            }
            return cotasVm;
        }

        public List<EventoRecursoViewModel> MapearEventoRecursoViewModel(List<EventoRecurso> recursos)
        {
            List<EventoRecursoViewModel> recursosVm = new List<EventoRecursoViewModel>();
            EventoRecursoViewModel vm;

            foreach (var eventoRecurso in recursos)
            {
                vm = new EventoRecursoViewModel(eventoRecurso.Id)
                {
                    RecursoId = eventoRecurso.Recurso.Id,
                    Nome = eventoRecurso.Recurso.Nome,
                    Custo = eventoRecurso.Recurso.Custo,
                    Quantidade = eventoRecurso.Quantidade
                };

                recursosVm.Add(vm);
            }
            return recursosVm;
        }

        public List<EventoHorarioViewModel> MapearEventoHorarioViewModel(List<EventoHorario> horarios)
        {
            List<EventoHorarioViewModel> horariosVm = new List<EventoHorarioViewModel>();
            EventoHorarioViewModel vm = null;

            foreach (var horario in horarios)
            {
                vm = new EventoHorarioViewModel(horario.Id);
                vm.DataInicio = horario.DataHoraInicio.Date;
                vm.HoraInicio = horario.DataHoraInicio;
                vm.DataFim = horario.DataHoraFim.Date;
                vm.HoraFim = horario.DataHoraFim;

                vm.DocenteId = horario.Docente.Id;
                vm.DocenteNome = horario.Docente.Pessoa.NomeSocial ?? horario.Docente.Pessoa.Nome;

                if (horario.Funcao != null)
                {
                    vm.FuncaoDocenteId = horario.Funcao.Id;
                    vm.FuncaoDocenteNome = horario.Funcao.Descricao;
                }

                vm.ModuloId = horario.Modulo.Id;
                vm.ModuloNome = horario.Modulo.Nome;

                if (horario.Sala != null)
                {
                    vm.SalaId = horario.Sala.Id;
                    vm.Sala = MapearSalaViewModel(horario.Sala);
                }

                horariosVm.Add(vm);
            }

            return horariosVm;
        }

        public EventoFinalizadoViewModel MapearEventoFinalizadoViewModel(Evento evento)
        {
            EventoFinalizadoViewModel vm = new EventoFinalizadoViewModel();

            if (evento == null)
            {
                return vm;
            }

            vm.Id = evento.Id;
            vm.Entidade = evento.EntidadeDemandante.Sigla + " - " + evento.EntidadeDemandante.Nome;
            vm.Nome = evento.Curso.Sigla + " - " + evento.Curso.Titulo;
            vm.DataInicio = evento.Horarios.Min(x => x.DataHoraInicio);
            vm.DataFim = evento.Horarios.Max(x => x.DataHoraFim);
            vm.DataInicioInscricao = evento.Agendas.Last().DataHoraInscricaoInicio;
            vm.DataFimInscricao = evento.Agendas.Last().DataHoraInscricaoFim;
            vm.PossuiCertificado = evento.Certificado != null;

            List<Inscrito> inscritos = evento.ListaDeInscricao.Inscritos.OrderBy(x => x.Cursista.Nome).ToList();

            InscricaoViewModel inscricaoVm;

            foreach (var inscrito in inscritos)
            {
                inscricaoVm = new InscricaoViewModel
                {
                    Id = inscrito.Id,
                    Cursista = MapearCursistaViewModel(inscrito.Cursista),
                    Situacao = inscrito.Situacao.ToString(),
                    Frequencia = inscrito.Frequencia
                };

                if (inscrito.Penalidade != null)
                {
                    inscricaoVm.Penalidade = new PenalidadeViewModel
                    {
                        Cancelada = inscrito.Penalidade.Cancelada(),
                        DataFim = inscrito.Penalidade.DataFimPenalidade,
                        DataInicio = inscrito.Penalidade.DataInicioPenalidade,
                        Id = inscrito.Penalidade.Id,
                        Justificativa = inscrito.Penalidade.JustificativaDeCancelamento,
                        DataDaPenalidade = inscrito.Penalidade.DataDaPenalidade
                    };
                }

                vm.Inscritos.Add(inscricaoVm);
            }
            vm.TotalInscritos = inscritos.Count();
            vm.TotalCertificados = inscritos.Count(x => x.Situacao == EnumSituacaoCursista.CERTIFICADO);
            vm.TotalDeclarados = inscritos.Count(x => x.Situacao == EnumSituacaoCursista.DECLARADO);
            vm.TotalReprovados = inscritos.Count(x => x.Situacao == EnumSituacaoCursista.DESISTENTE);

            vm.Docentes = evento.Horarios.GroupBy(x => x.Docente)
                .Select(x => new DocenteViewModel
                {
                    Cpf = x.Key.Pessoa.Cpf,
                    Id = x.Key.Id,
                    Nome = x.Key.Pessoa.Nome,
                    NumeroFuncional = x.Key.Pessoa.NumeroFuncional
                }).ToList();

            return vm;
        }

        public ListaInscritosViewModel MapearListaInscritosViewModel(ListaDeInscricao lista)
        {
            ListaInscritosViewModel listaInscritos = new ListaInscritosViewModel();

            if (lista == null)
            {
                return listaInscritos;
            }

            listaInscritos.Evento.Id = lista.Evento.Id;
            listaInscritos.Evento.Entidade = lista.Evento.EntidadeDemandante.Sigla + " - " + lista.Evento.EntidadeDemandante.Nome;
            listaInscritos.Evento.Nome = lista.Evento.Curso.Sigla + " - " + lista.Evento.Curso.Titulo;
            listaInscritos.Evento.DataInicio = lista.Evento.Horarios.Min(x => x.DataHoraInicio);
            listaInscritos.Evento.DataFim = lista.Evento.Horarios.Max(x => x.DataHoraFim);
            listaInscritos.Evento.DataInicioInscricao = lista.Evento.Agendas.Last().DataHoraInscricaoInicio;
            listaInscritos.Evento.DataFimInscricao = lista.Evento.Agendas.Last().DataHoraInscricaoFim;

            if (lista.Evento.ListaDeInscricao != null)
            {
                List<Inscrito> inscritos = lista.Inscritos.OrderBy(x => x.Cursista.Nome).ToList();

                foreach (var inscrito in inscritos)
                {
                    if (!inscrito.DeletionTime.HasValue)
                    {
                        listaInscritos.Inscritos.Add(new InscricaoViewModel()
                        {
                            Id = inscrito.Id,
                            Cursista = MapearCursistaViewModel(inscrito.Cursista),
                            DataDeInscricao = inscrito.DataDeInscricao
                        });
                    }
                }
            }
            else
            {
                listaInscritos.Inscritos = new List<InscricaoViewModel>();
            }

            return listaInscritos;
        }

        public ListaInscritosViewModel MapearEventoListaInscritosViewModel(Evento evento)
        {
            ListaInscritosViewModel listaInscritos = new ListaInscritosViewModel();
            listaInscritos.Evento.Id = evento.Id;
            listaInscritos.Evento.Entidade = evento.EntidadeDemandante.Sigla + " - " + evento.EntidadeDemandante.Nome;
            listaInscritos.Evento.Nome = evento.Curso.Sigla + " - " + evento.Curso.Titulo;
            listaInscritos.Evento.DataInicio = evento.Horarios.Min(x => x.DataHoraInicio);
            listaInscritos.Evento.DataFim = evento.Horarios.Max(x => x.DataHoraFim);
            listaInscritos.Evento.DataInicioInscricao = evento.Agendas.Last().DataHoraInscricaoInicio;
            listaInscritos.Evento.DataFimInscricao = evento.Agendas.Last().DataHoraInscricaoFim;

            if (evento.ListaDeInscricao != null)
            {
                foreach (var inscrito in evento.ListaDeInscricao.Inscritos)
                {
                    if (!inscrito.DeletionTime.HasValue)
                    {
                        listaInscritos.Inscritos.Add(new InscricaoViewModel()
                        {
                            Id = inscrito.Id,
                            Cursista = MapearCursistaViewModel(inscrito.Cursista),
                            DataDeInscricao = inscrito.DataDeInscricao
                        });
                    }
                }
            }
            else
            {
                listaInscritos.Inscritos = new List<InscricaoViewModel>();
            }

            return listaInscritos;
        }

        public CursistaViewModel MapearCursistaViewModel(Pessoa pessoa)
        {
            CursistaViewModel cursista = new CursistaViewModel
            {
                Id = pessoa.Id,
                Nome = pessoa.NomeSocial ?? pessoa.Nome,
                Cpf = FormatadorDeDados.FormatarCPF(pessoa.Cpf),
                Email = pessoa.Email,
                NumeroFuncional = pessoa.NumeroFuncional
            };

            return cursista;
        }

        public List<CursistaViewModel> MapearCursistasViewModel(Evento evento, List<Pessoa> pessoas)
        {
            List<CursistaViewModel> listaCursistas = new List<CursistaViewModel>();

            foreach (var pessoa in pessoas)
            {
                listaCursistas.Add(new CursistaViewModel
                {
                    Id = pessoa.Id,
                    Nome = pessoa.NomeSocial ?? pessoa.Nome,
                    Cpf = FormatadorDeDados.FormatarCPF(pessoa.Cpf),
                    Email = pessoa.Email,
                    NumeroFuncional = pessoa.NumeroFuncional,
                    Inscrito = evento.ListaDeInscricao.EstaInscrito(pessoa),
                    Entidade = pessoa.Entidade.Sigla
                });
            }

            return listaCursistas;
        }

        public List<ListaPresencaInscritosViewModel> MapearListaPresencaEvento(Evento evento)
        {
            List<ListaPresencaInscritosViewModel> listaPresenca = new List<ListaPresencaInscritosViewModel>();
            if (evento.ListaDeInscricao != null)
            {
                List<Inscrito> lista = evento.ListaDeInscricao.Inscritos.Where(x => !x.DeletionTime.HasValue).ToList();
                //foreach (var inscritos in evento.ListaDeInscricao.Inscritos)
                foreach (var inscritos in lista)
                {
                    listaPresenca.Add(new ListaPresencaInscritosViewModel
                    {
                        PessoaId = inscritos.Cursista.Id,
                        PessoaNome = inscritos.Cursista.NomeSocial ?? inscritos.Cursista.Nome
                    });
                }
            }

            return listaPresenca;
        }

        public List<DropDownViewModel> MapearDropDownViewModel(List<FuncaoDocente> funcoes)
        {
            var dropdownVm = new List<DropDownViewModel>();

            foreach (var funcao in funcoes)
            {
                dropdownVm.Add(new DropDownViewModel
                {
                    Id = funcao.Id,
                    Nome = funcao.Descricao
                });
            }

            return dropdownVm;
        }
    }
}
