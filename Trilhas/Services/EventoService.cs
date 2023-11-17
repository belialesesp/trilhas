using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trilhas.Data;
using Trilhas.Data.Enums;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Data.Model.Eventos;
using Trilhas.Data.Model.Exceptions;
using Trilhas.Helper.Contract;
using Trilhas.Models.Relatorio;

namespace Trilhas.Services
{
    public class EventoService
    {
        private ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public EventoService(ApplicationDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public int QuantidadeDeEventos(long cursoId,
                                    string cursoNome,
                                    EnumModalidade? modalidade,
                                    long entidadeDemandanteId,
                                    string entidadeNome,
                                    long municipioId,
                                    long docenteId,
                                    string docenteNome,
                                    long cursistaId,
                                    DateTime dataInicio,
                                    DateTime dataFim,
                                    bool naoIniciados,
                                    bool andamentos,
                                    bool concluidos,
                                    bool cancelado,
                                    bool inscricao,
                                    bool finalizados
                                    )
        {
            return PesquisarEvento(cursoId, cursoNome, modalidade, entidadeDemandanteId, entidadeNome, municipioId, docenteId, docenteNome, cursistaId, dataInicio, dataFim, naoIniciados, andamentos, concluidos, cancelado, inscricao, finalizados).Count();
        }

        public List<Evento> PesquisarEventos(long cursoId,
                                    string cursoNome,
                                    EnumModalidade? modalidade,
                                    long entidadeDemandanteId,
                                    string entidadeNome,
                                    long municipioId,
                                    long docenteId,
                                    string docenteNome,
                                    long cursistaId,
                                    DateTime dataInicio,
                                    DateTime dataFim,
                                    bool naoIniciados,
                                    bool andamentos,
                                    bool concluidos,
                                    bool cancelados,
                                    bool inscricao,
                                    bool finalizados,
                                    int start = -1, int count = -1)
        {
            return PesquisarEvento(cursoId, cursoNome, modalidade, entidadeDemandanteId, entidadeNome, municipioId, docenteId, docenteNome, cursistaId, dataInicio, dataFim, naoIniciados, andamentos, concluidos, cancelados, inscricao, finalizados, start, count).ToList();
        }

        private IQueryable<Evento> PesquisarEvento(long cursoId,
                            string cursoNome,
                            EnumModalidade? modalidade,
                            long entidadeDemandanteId,
                            string entidadeNome,
                            long municipioId,
                            long docenteId,
                            string docenteNome,
                            long cursistaId,
                            DateTime dataInicio,
                            DateTime dataFim,
                            bool naoIniciados,
                            bool andamentos,
                            bool concluidos,
                            bool cancelados,
                            bool inscricao,
                            bool finalizados,
                            int start = -1, int count = -1)
        {
            IQueryable<Evento> result = _context.Eventos
            .Include(x => x.Agendas)
            .Include(x => x.Coordenador)
            .Include(x => x.Curso)
            .Include(x => x.EntidadeDemandante)
            .Include(x => x.Local).ThenInclude(y => y.Municipio)
            .Include(x => x.Cotas).ThenInclude(a => a.Entidade)
            .Include(x => x.Curso).ThenInclude(b => b.Modulos)
            .Include(x => x.Horarios).ThenInclude(d => d.Docente).ThenInclude(p => p.Pessoa)
            .Include(x => x.Horarios).ThenInclude(d => d.Modulo)
            .Include(x => x.Recursos).ThenInclude(e => e.Recurso)
            .Include(x => x.ListaDeInscricao).ThenInclude(f => f.Inscritos).ThenInclude(g => g.Cursista);

            if (!(naoIniciados && andamentos && concluidos && inscricao && cancelados && finalizados))
            {
                result = result.Where(x => (naoIniciados && !x.DeletionTime.HasValue && x.Agendas.Any(y => y.DataHoraInicio >= DateTime.Now) && !x.Agendas.Any(y => y.DataHoraInscricaoInicio <= DateTime.Now && y.DataHoraInscricaoFim >= DateTime.Now)) ||
                                           (andamentos && !x.DeletionTime.HasValue && x.Agendas.Any(y => (y.DataHoraInicio <= DateTime.Now) && (y.DataHoraFim >= DateTime.Now))) ||
                                           (concluidos && !x.DeletionTime.HasValue && !x.Finalizado && x.Agendas.Any(y => y.DataHoraFim <= DateTime.Now)) ||
                                           (finalizados && !x.DeletionTime.HasValue && x.Finalizado && x.Agendas.Any(y => y.DataHoraFim <= DateTime.Now)) ||
                                           (inscricao && !x.DeletionTime.HasValue && x.Agendas.Any(y => y.DataHoraInscricaoInicio <= DateTime.Now && y.DataHoraInscricaoFim >= DateTime.Now)) ||
                                           (cancelados && x.DeletionTime.HasValue));
            }

            if (cursoId > 0)
            {
                result = result.Where(x => x.Curso.Id == cursoId);
            }
            if (!string.IsNullOrEmpty(cursoNome))
            {
                result = result.Where(x => x.Curso.Titulo.ToUpper().Contains(cursoNome.ToUpper()) || x.Curso.Sigla.ToUpper().Contains(cursoNome.ToUpper()));
            }
            if (modalidade.HasValue)
            {
                result = result.Where(x => x.Curso.Modalidade == modalidade);
            }
            if (entidadeDemandanteId > 0)
            {
                result = result.Where(x => (x.EntidadeDemandante.Id == entidadeDemandanteId));
            }
            if (!string.IsNullOrEmpty(entidadeNome))
            {
                result = result.Where(x => x.EntidadeDemandante.Nome.ToUpper().Contains(entidadeNome.ToUpper()) || x.EntidadeDemandante.Sigla.ToUpper().Contains(entidadeNome.ToUpper()));
            }
            if (docenteId > 0)
            {
                result = result.Where(x => (x.Horarios.Any(y => y.Docente.Id == docenteId)));
            }
            if (!string.IsNullOrEmpty(docenteNome))
            {
                result = result.Where(y => y.Horarios.Any(x => x.Docente.Pessoa.Nome.ToUpper().Contains(docenteNome.ToUpper())));
            }
            if (cursistaId > 0)
            {
                result = result.Where(x => x.ListaDeInscricao.Inscritos.Any(y => y.Cursista.Id == cursistaId && !y.DeletionTime.HasValue));
            }
            if (municipioId > 0)
            {
                result = result.Where(x => x.Local.Municipio.Id == municipioId);
            }
            if (dataInicio != DateTime.MinValue)
            {
                result = result.Where(x => x.Agendas.Any(o => o.DataHoraInicio >= dataInicio));
            }
            if (dataFim != DateTime.MaxValue)
            {
                result = result.Where(x => x.Agendas.Any(o => o.DataHoraInicio <= dataFim));
            }

            result = result.OrderBy(x => x.Id);

            if (start > 0)
            {
                result = result.Skip(start);
            }
            if (count > 0)
            {
                result = result.Take(count);
            }

            return result;
        }

        public Evento RecuperarEventoEdicao(long id, bool incluirExcluidos = false)
        {
            return _context.Eventos
            .Include(x => x.Coordenador)
            .Include(x => x.EntidadeDemandante)
            .Include(x => x.Agendas)
            .Include(x => x.Curso).ThenInclude(x => x.Modulos)
            .Include(x => x.Local).ThenInclude(x => x.Municipio)
            .Include(x => x.Local).ThenInclude(x => x.Salas)
            .Include(x => x.Recursos).ThenInclude(x => x.Recurso)
            .Include(x => x.Horarios).ThenInclude(x => x.Modulo)
            .Include(x => x.Horarios).ThenInclude(x => x.Docente).ThenInclude(x => x.Pessoa)
            .Include(x => x.Horarios).ThenInclude(x => x.Funcao)
            .Include(x => x.Horarios).ThenInclude(x => x.Sala)
            .Include(x => x.Cotas).ThenInclude(c => c.Entidade)
            .Include(x => x.ListaDeInscricao)
            .Include(x => x.Certificado)
            .Include(x => x.DeclaracaoCursista)
            .Include(x => x.DeclaracaoDocente)
            .FirstOrDefault(x => x.Id == id && (!x.DeletionTime.HasValue || incluirExcluidos));
        }

        public Evento RecuperarEventoInscricao(long id, bool incluirExcluidos = false)
        {
            return _context.Eventos
                .Include(x => x.EntidadeDemandante)
                .Include(x => x.Agendas)
                .Include(x => x.Cotas).ThenInclude(x => x.Entidade)
                .Include(x => x.ListaDeInscricao).ThenInclude(x => x.Inscritos).ThenInclude(x => x.Penalidade)
                .Include(x => x.ListaDeInscricao).ThenInclude(x => x.Inscritos).ThenInclude(x => x.Cursista).ThenInclude(x => x.Entidade)
                .FirstOrDefault(x => x.Id == id && (!x.DeletionTime.HasValue || incluirExcluidos));
        }

        public Evento RecuperarEventoListaPresenca(long id)
        {
            return _context.Eventos
                .Include(x => x.Curso)
                .Include(x => x.EntidadeDemandante)
                .Include(x => x.Agendas)
                .Include(x => x.Local)
                .Include(x => x.Horarios).ThenInclude(x => x.Modulo)
                .Include(x => x.ListaDeInscricao).ThenInclude(x => x.Inscritos).ThenInclude(x => x.Cursista).ThenInclude(x => x.Entidade)
                .Include(x => x.ListaDeInscricao).ThenInclude(x => x.Inscritos).ThenInclude(x => x.Cursista.Contatos)
                .Include(x => x.ListaDeInscricao.Inscritos).ThenInclude(x => x.Cursista).ThenInclude(x => x.Municipio)
                .FirstOrDefault(x => x.Id == id && !x.DeletionTime.HasValue);
        }

        public Evento RecuperarEventoCompleto(long id, bool incluirExcluidos = false)
        {
            return _context.Eventos
                .Include(x => x.Certificado)
                .Include(x => x.DeclaracaoCursista)
                .Include(x => x.DeclaracaoDocente)
                .Include(x => x.Coordenador)
                .Include(x => x.EntidadeDemandante)
                .Include(x => x.Agendas)
                .Include(x => x.Curso).ThenInclude(x => x.Modulos)
                .Include(x => x.Local).ThenInclude(x => x.Municipio)
                .Include(x => x.Local).ThenInclude(x => x.Salas)
                .Include(x => x.Recursos).ThenInclude(x => x.Recurso)
                .Include(x => x.Horarios).ThenInclude(x => x.Modulo)
                .Include(x => x.Horarios).ThenInclude(x => x.Docente).ThenInclude(x => x.Habilitacao).ThenInclude(x => x.Curso)
                .Include(x => x.Horarios).ThenInclude(x => x.Docente).ThenInclude(x => x.Pessoa)
                .Include(x => x.Horarios).ThenInclude(x => x.Sala)
                .Include(x => x.Horarios).ThenInclude(x => x.ListaDePresenca)
                .Include(x => x.Cotas).ThenInclude(c => c.Entidade)
                .Include(x => x.ListaDeInscricao).ThenInclude(x => x.Inscritos).ThenInclude(x => x.Cursista)
                .Include(x => x.ListaDeInscricao).ThenInclude(x => x.Inscritos).ThenInclude(x => x.Penalidade)
               .FirstOrDefault(x => x.Id == id && (!x.DeletionTime.HasValue || incluirExcluidos));
        }

        public Evento SalvarEvento(string userId, Evento evento)
        {
            ValidarPossibilidadeDeEdicao(evento);

            ValidarHorarios(evento);
            ValidarCotas(evento);

            if (evento.Id > 0)
            {
                _context.EventoRecurso.RemoveRange(_context.EventoRecurso.Include(x => x.Evento).Where(x => x.Evento.Id == evento.Id && !evento.Recursos.Any(y => y.Id == x.Id)));
                _context.EventoHorario.RemoveRange(_context.EventoHorario.Include(x => x.Evento).Where(x => x.Evento.Id == evento.Id && !evento.Horarios.Any(y => y.Id == x.Id)));
                _context.EventoCota.RemoveRange(_context.EventoCota.Include(x => x.Evento).Where(x => x.Evento.Id == evento.Id && !evento.Cotas.Any(y => y.Id == x.Id)));

                evento.LastModifierUserId = userId;
                evento.LastModificationTime = DateTime.Now;

                evento.Agenda().LastModifierUserId = userId;
                evento.Agenda().LastModificationTime = DateTime.Now;
            }
            else
            {
                evento.CreatorUserId = userId;
                evento.CreationTime = DateTime.Now;

                evento.Agenda().CreatorUserId = userId;
                evento.Agenda().CreationTime = DateTime.Now;

                if (evento.ListaDeInscricao == null)
                {
                    evento.ListaDeInscricao = new ListaDeInscricao
                    {
                        CreatorUserId = userId,
                        CreationTime = DateTime.Now
                    };
                }

                _context.Eventos.Add(evento);
            }

            _context.SaveChanges();

            EnviarNotificacaoDeNovoEvento(evento);

            return evento;
        }

        private void ValidarPossibilidadeDeEdicao(Evento evento)
        {
            var situacao = evento.Situacao();

            if (situacao == SituacaoEvento.Finalizado)
            {
                throw new TrilhasException("Esse Evento já foi Finalizado e não pode mais ser alterado.");
            }
            else if (situacao == SituacaoEvento.Cancelado)
            {
                throw new TrilhasException("Esse Evento foi Cancelado e não pode mais ser alterado.");
            }
        }

        private void ValidarHorarios(Evento evento)
        {
            if (evento.Horarios == null || evento.Horarios.Count == 0)
            {
                throw new TrilhasValidationException("Nenhum horário foi definido.");
            }

            SepararHorariosPorDia(evento);
            VerificarConflitos(evento);

            VerificarDataDeInscricao(evento);
            VerificarHorariosParaTodosModulos(evento);
            VerificarCargaHoraria(evento);

            foreach (var horario in evento.Horarios)
            {
                VerificarDisponiblidadeDoDocente(horario);
                VerificarDisponibilidadeDaSala(horario);
            }
        }

        private void SepararHorariosPorDia(Evento evento)
        {
            List<EventoHorario> novosHorarios = new List<EventoHorario>();
            EventoHorario horario, novoHorario;
            DateTime dataHoraAux;

            for (var x = 0; x < evento.Horarios.Count; x++)
            {
                horario = evento.Horarios[x];

                if (horario.DataHoraInicio.Date < horario.DataHoraFim.Date)
                {
                    dataHoraAux = horario.DataHoraInicio;

                    while (dataHoraAux.Date <= horario.DataHoraFim.Date)
                    {
                        if (dataHoraAux.DayOfWeek == DayOfWeek.Saturday || dataHoraAux.DayOfWeek == DayOfWeek.Sunday)
                        {
                            dataHoraAux = dataHoraAux.AddDays(1);
                            continue;
                        }

                        novoHorario = new EventoHorario
                        {
                            DataHoraInicio = dataHoraAux,
                            DataHoraFim = dataHoraAux.Date.Add(horario.DataHoraFim.TimeOfDay),
                            Docente = horario.Docente,
                            Funcao = horario.Funcao,
                            Modulo = horario.Modulo,
                            Sala = horario.Sala,
                            Evento = horario.Evento
                        };

                        novosHorarios.Add(novoHorario);

                        dataHoraAux = dataHoraAux.AddDays(1);
                    }

                    evento.Horarios.Remove(horario);
                    x--;
                }
            }

            evento.Horarios.AddRange(novosHorarios);
        }

        private void VerificarCargaHoraria(Evento evento)
        {
            DateTime horario, horarioInicio, horarioFim;
            double somatorio;

            foreach (var modulo in evento.Curso.Modulos)
            {
                var horariosModulo = evento.Horarios.Where(x => x.Modulo.Id == modulo.Id).ToList();

                somatorio = 0;

                for (var x = 0; x < horariosModulo.Count; x++)
                {
                    horario = horariosModulo[x].DataHoraInicio;
                    horarioInicio = new DateTime(horario.Year, horario.Month, horario.Day, horario.Hour, 0, 0, horario.Kind);
                    horario = horariosModulo[x].DataHoraFim;
                    horarioFim = new DateTime(horario.Year, horario.Month, horario.Day, horario.Hour, 0, 0, horario.Kind);

                    var diff = horarioFim - horarioInicio;

                    somatorio += diff.TotalHours;
                }

                if (somatorio != modulo.CargaHoraria)
                {
                    throw new TrilhasException("O total de horas não coincide com a carga horária cadastrada no módulo: " + modulo.Nome + ".");
                }
            }
        }

        private void VerificarConflitos(Evento evento)
        {
            EventoHorario h1, h2;

            for (var x = 0; x < evento.Horarios.Count; x++)
            {
                h1 = evento.Horarios[x];

                VerificarHorarioDeInicioFim(h1);

                for (var y = (x + 1); y < evento.Horarios.Count; y++)
                {
                    h2 = evento.Horarios[y];

                    var conflito = ((h2.DataHoraInicio >= h1.DataHoraInicio && h2.DataHoraInicio < h1.DataHoraFim) ||
                                   (h2.DataHoraFim > h1.DataHoraInicio && h2.DataHoraFim < h1.DataHoraFim)) &&
                                   (h1.Docente.Id == h2.Docente.Id || (h1.Sala != null && h2.Sala != null && h1.Sala.Id == h2.Sala.Id));

                    if (conflito)
                    {
                        throw new TrilhasValidationException("Existem horários conflitantes.");
                    }
                }
            }
        }

        private void VerificarDataDeInscricao(Evento evento)
        {
            EventoAgenda agenda = evento.Agenda();

            if (agenda.DataHoraInscricaoInicio >= agenda.DataHoraInscricaoFim)
            {
                throw new TrilhasValidationException("Data/Hora de inscrição inválidos.");
            }

            if (agenda.DataHoraInscricaoFim >= evento.Horarios.Min(x => x.DataHoraInicio))
            {
                throw new TrilhasValidationException("A Data/Hora de inscrição deve ser anterior à data de início do Evento.");
            }
        }

        private void VerificarHorariosParaTodosModulos(Evento evento)
        {
            var todosModulos = evento.Curso.Modulos.All(x => evento.Horarios.Any(y => y.Modulo.Id == x.Id));

            if (!todosModulos)
            {
                throw new TrilhasValidationException("Não foram definidos horários para todos os Módulos do Curso.");
            }
        }

        private void VerificarHorarioDeInicioFim(EventoHorario horario)
        {
            if (horario.DataHoraInicio.Date != horario.DataHoraFim.Date)
            {
                throw new TrilhasValidationException("Horários inválidos.");
            }

            if (horario.DataHoraInicio >= horario.DataHoraFim)
            {
                throw new TrilhasValidationException("A data/hora de início deve ser anterior a data/hora fim.");
            }
        }

        private void VerificarDisponiblidadeDoDocente(EventoHorario horario)
        {
            if (horario.Docente == null)
            {
                throw new TrilhasValidationException("Escolha o Docente para cada Horário cadastrado.");
            }

            if (HorarioRetroativo(horario))
            {
                return;
            }

            var ocupado = _context.EventoHorario
                .Include(x => x.Evento).ThenInclude(x => x.Curso)
                .Include(x => x.Docente).ThenInclude(x => x.Pessoa)
                .FirstOrDefault(x => (horario.Evento.Id <= 0 || horario.Evento.Id != x.Evento.Id) && x.Docente.Id == horario.Docente.Id &&
                (
                    (horario.DataHoraInicio >= x.DataHoraInicio && horario.DataHoraInicio < x.DataHoraFim) ||
                    (horario.DataHoraFim > x.DataHoraInicio && horario.DataHoraFim < x.DataHoraFim)
                ));

            if (ocupado != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("O Docente ")
                    .Append(horario.Docente.Pessoa.Nome)
                    .Append(" já está alocado para o Curso ").Append(ocupado.Evento.Curso.Titulo).Append(" no horário escolhido.");

                throw new TrilhasValidationException(sb.ToString());
            }
        }

        private void VerificarDisponibilidadeDaSala(EventoHorario horario)
        {
            if (HorarioRetroativo(horario) || horario.Evento.Curso.Modalidade == EnumModalidade.EAD)
            {
                return;
            }

            var reservado = _context.EventoHorario
                .Include(x => x.Evento).ThenInclude(x => x.Curso)
                .Include(x => x.Sala)
                .FirstOrDefault(x => (horario.Evento.Id <= 0 || horario.Evento.Id != x.Evento.Id) && x.Sala.Id == horario.Sala.Id &&
                (
                    (horario.DataHoraInicio >= x.DataHoraInicio && horario.DataHoraInicio < x.DataHoraFim) ||
                    (horario.DataHoraFim > x.DataHoraInicio && horario.DataHoraFim < x.DataHoraFim)
                ));

            if (reservado != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("A Sala ")
                    .Append(horario.Sala.Sigla).Append("-").Append(horario.Sala.Numero)
                    .Append(" já está reservada para o Curso ").Append(reservado.Evento.Curso.Titulo).Append(" no horário escolhido.");

                throw new TrilhasValidationException(sb.ToString());
            }
        }

        private bool HorarioRetroativo(EventoHorario horario)
        {
            return horario.DataHoraInicio <= DateTime.Now;
        }

        private void ValidarCotas(Evento evento)
        {
            AgruparCotasPorEntidade(evento);
            VerificarTotalDeCotas(evento);
        }

        private void AgruparCotasPorEntidade(Evento evento)
        {
            List<EventoCota> cotas = new List<EventoCota>();

            foreach (var cota in evento.Cotas)
            {
                var item = cotas.FirstOrDefault(x => x.Entidade.Id == cota.Entidade.Id);

                if (item == null)
                {
                    cotas.Add(cota);
                }
                else
                {
                    item.Quantidade += cota.Quantidade;
                }
            }

            evento.Cotas = cotas;
        }

        private void VerificarTotalDeCotas(Evento evento)
        {
            if (evento.Cotas.Sum(x => x.Quantidade) > evento.QuantidadeDeVagas())
            {
                throw new TrilhasValidationException("Quantidade de cotas é superior à quantidade de vagas do Evento.");
            }
        }

        private void EnviarNotificacaoDeNovoEvento(Evento evento)
        {
            //TODO
            //_context.ItensDaTrilha.Where(x => x.SolucaoEducacional.Id == evento.Curso.Id).Select(x => x.Trilha.)
        }

        public void ExcluirEvento(string userId, long id, string motivoExclusao)
        {
            Evento evento = RecuperarEventoInscricao(id);

            if (evento == null)
            {
                throw new RecordNotFoundException("Registro não encontrado.");
            }

            var situacao = evento.Situacao();
            if (situacao == SituacaoEvento.Encerrado || situacao == SituacaoEvento.Finalizado)
            {
                throw new ConstraintException("O Evento não pode ser cancelado porque já está encerrado.");
            }

            DateTime agora = DateTime.Now;

            evento.ListaDeInscricao.DeletionTime = agora;
            evento.ListaDeInscricao.DeletionUserId = userId;
            evento.MotivoExclusao = motivoExclusao;

            var inscritos = evento.ListaDeInscricao.Inscritos.Where(x => !x.DeletionTime.HasValue).ToList();

            foreach (var inscricao in inscritos)
            {
                inscricao.DeletionTime = agora;
                inscricao.DeletionUserId = userId;
            }

            evento.DeletionTime = agora;
            evento.DeletionUserId = userId;

            _context.SaveChanges();
        }

        public EventoHorario RecuperarEventoHorario(long idEventoHorario)
        {
            return _context.EventoHorario
                .Include(x => x.Evento)
                .Include(x => x.Docente)
                .Include(x => x.Modulo)
                .FirstOrDefault(x => x.Id == idEventoHorario);
        }

        public ListaDeInscricao RecuperarListaInscricao(long id)
        {
            return _context.ListaDeInscricao
                .Include(x => x.Evento).ThenInclude(x => x.Curso)
                .Include(x => x.Evento).ThenInclude(x => x.EntidadeDemandante)
                .Include(x => x.Evento).ThenInclude(x => x.Horarios)
                .Include(x => x.Evento).ThenInclude(x => x.Agendas)
                .Include(y => y.Inscritos).ThenInclude(z => z.Cursista)
                .Include(y => y.Inscritos).ThenInclude(z => z.Cursista.Entidade)
                .FirstOrDefault(x => x.Id == id);
        }

        public Inscrito RecuperarInscricao(long id)
        {
            return _context.Inscritos
                .Include(x => x.Cursista)
                .Include(x => x.ListaDeInscricao).ThenInclude(x => x.Evento).ThenInclude(x => x.Local)
                .Include(x => x.ListaDeInscricao).ThenInclude(x => x.Evento).ThenInclude(x => x.Certificado)
                .Include(x => x.ListaDeInscricao).ThenInclude(x => x.Evento).ThenInclude(x => x.DeclaracaoCursista)
                .Include(x => x.ListaDeInscricao).ThenInclude(x => x.Evento).ThenInclude(x => x.Agendas)
                .Include(x => x.ListaDeInscricao).ThenInclude(x => x.Evento).ThenInclude(x => x.Horarios).ThenInclude(x => x.Docente).ThenInclude(x => x.Pessoa)
                .Include(x => x.ListaDeInscricao).ThenInclude(x => x.Evento).ThenInclude(x => x.Curso).ThenInclude(x => x.Modulos)
                .Include(x => x.Penalidade)
                .FirstOrDefault(x => x.Id == id);
        }

        public bool InscreverCursista(string userId, Evento evento, Pessoa cursista, DateTime dataInscricao)
        {
            if (evento == null || cursista == null)
            {
                return false;
            }

            VerificarPenalidades(cursista);

            Inscrito inscricao = evento.InscreverCursista(cursista, DateTime.Now);

            if (inscricao != null)
            {
                inscricao.CreationTime = DateTime.Now;
                inscricao.CreatorUserId = userId;

                _context.Inscritos.Add(inscricao);
                _context.SaveChanges();

                return true;
            }

            return false;
        }

        private void VerificarPenalidades(Pessoa cursista)
        {
            Penalidade penalidade = _context.Penalidades.FirstOrDefault(x => x.Cursista.Id == cursista.Id && x.DataFimPenalidade.Date >= DateTime.Now.Date);

            if (penalidade != null && !penalidade.Cancelada())
            {
                throw new TrilhasException("O Cursista possui uma penalidade e não pode ser inscrito em Eventos até o dia " + penalidade.DataFimPenalidade.ToString("dd/MM/yyyy") + ".");
            }
        }

        public bool CancelarInscricao(string userId, Evento evento, Pessoa pessoa)
        {
            var retorno = evento.CancelarInscricao(pessoa, userId);

            _context.SaveChanges();

            return retorno;
        }

        public void FinalizarEvento(string userId, Evento evento, int prazoPenalidade)
        {
            evento.Finalizar(prazoPenalidade);

            evento.LastModificationTime = DateTime.Now;
            evento.LastModifierUserId = userId;

            AdicionarHabilitacaoDocentes(evento);

            _context.SaveChanges();
        }

        public void AlterarFrequencia(Evento evento, long cursistaId)
        {
            evento.CalcularFrequencias(cursistaId);
            var inscrito = evento.ListaDeInscricao.Inscritos.FirstOrDefault(x => x.Cursista.Id == cursistaId);

            if (inscrito.Situacao != EnumSituacaoCursista.DESISTENTE && inscrito.Penalidade != null)
            {
                inscrito.Penalidade.JustificativaDeCancelamento = "Frequência do cursista alterada";
                SalvarPenalidade(inscrito.Penalidade);
            }
            else
            {
                if (inscrito.Penalidade == null)
                {
                    int prazoPenalidade = _configuration.GetValue<int>("Parametros:PrazoPenalidade");
                    evento.GerarPenalidades(prazoPenalidade);
                }
                else
                {
                    inscrito.Penalidade.JustificativaDeCancelamento = null;
                    SalvarPenalidade(inscrito.Penalidade);
                }
            }

            _context.SaveChanges();
        }

        private void AdicionarHabilitacaoDocentes(Evento evento)
        {
            var docentes = evento.Horarios.Select(x => x.Docente).Distinct();

            foreach (var docente in docentes)
            {
                docente.AdicionarHabilitacao(evento.Curso);
            }
        }

        public void SalvarPenalidade(Penalidade input)
        {
            var penalidade = new Penalidade();

            if (input.Id > 0)
            {
                penalidade = _context.Penalidades.FirstOrDefault(x => x.Id == input.Id);

            }
            else
            {
                penalidade.Cursista = input.Cursista;
            }

            penalidade.DataDaPenalidade = input.DataDaPenalidade;
            penalidade.DataFimPenalidade = input.DataFimPenalidade;
            penalidade.DataInicioPenalidade = input.DataInicioPenalidade;
            penalidade.JustificativaDeCancelamento = input.JustificativaDeCancelamento;

            _context.SaveChanges();
        }

        public List<Evento> PesquisarProgramacaoEventos(DateTime dataInicio, DateTime dataFim)
        {
            IQueryable<Evento> result = _context.Eventos
            .Include(x => x.Agendas)
            .Include(x => x.Coordenador)
            .Include(x => x.Curso).ThenInclude(y => y.Estacao).ThenInclude(z => z.Eixo)
            .Include(x => x.Local).ThenInclude(y => y.Municipio)
            .Include(x => x.Cotas).ThenInclude(a => a.Entidade)
            .Include(x => x.Curso).ThenInclude(b => b.Modulos)
            .Include(x => x.Horarios).ThenInclude(c => c.Docente).ThenInclude(p => p.Pessoa)
            .Include(x => x.Horarios).ThenInclude(d => d.Modulo);

            result = result.Where(x => x.Agendas.Any(y => (y.DataHoraInicio >= dataInicio) && (y.DataHoraInicio <= dataFim) && !y.DeletionTime.HasValue) && !x.DeletionTime.HasValue);

            //result = result.OrderBy(x => x.Agendas.Last().DataHoraInicio);

            return result.ToList();
        }

        public List<FuncaoDocente> RecuperarFuncoesDocente()
        {
            return _context.FuncoesDocente.OrderBy(x => x.Descricao).ToList();
        }

        public FuncaoDocente RecuperarFuncaoDocente(long id)
        {
            return _context.FuncoesDocente.FirstOrDefault(x => x.Id == id);
        }

        public List<Inscrito> RecuperaEventoInscricao(long idCursista)
        {
            var inscritos = _context.Inscritos
                .Include(x => x.Cursista)
                .Include(x => x.ListaDeInscricao).ThenInclude(x => x.Evento).ThenInclude(x => x.Curso).ThenInclude(x => x.Estacao).ThenInclude(x => x.Eixo)
                .Where(x => x.Id == idCursista && (x.Situacao == EnumSituacaoCursista.CERTIFICADO || x.Situacao == EnumSituacaoCursista.DECLARADO))
                .OrderBy(x => x.Cursista.Nome)
                .ToList();

            return inscritos;
        }

        public List<Evento> RecuperaEventoCursista(long cursistaId)
        {
            var eventos = _context.Eventos
                .Include(x => x.Horarios).ThenInclude(y => y.Docente).ThenInclude(z => z.Pessoa)
                .Include(x => x.Curso)
                .Include(x => x.EntidadeDemandante)
                .Include(x => x.Agendas)
                .Include(x => x.ListaDeInscricao.Inscritos).ThenInclude(h => h.Cursista)
                .Where(x => (x.ListaDeInscricao.Inscritos.Any(y => (y.Cursista.Id == cursistaId) && (!x.DeletionTime.HasValue)))
                    && (x.Finalizado)
                    && (!x.DeletionTime.HasValue))
                .OrderBy(x => x.Curso.Titulo)
                .ToList();

            return eventos;
        }

    }
}
