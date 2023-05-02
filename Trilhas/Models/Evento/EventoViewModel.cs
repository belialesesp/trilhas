using System.Collections.Generic;

namespace Trilhas.Models.Evento
{
    public class EventoViewModel
    {
        public long Id { get; set; }

        public EventoCoordenadorViewModel Coordenador { get; set; }

        public EventoCursoViewModel Curso { get; set; }

        public EventoEntidadeViewModel Entidade { get; set; }

        public EventoLocalViewModel Local { get; set; }

        public string Observacoes { get; set; }
        public bool FlagEad { get; set; }
        public string UrlEad { get; set; }
        public string Situacao { get; set; }
        public bool LimitarVagas { get; set; }
        public int VagasPorEntidade { get; set; }
        public long? CertificadoId { get; set; }
        public long? DeclaracaoCursistaId { get; set; }
        public long? DeclaracaoDocenteId { get; set; }
        public long ListaDeInscricaoId { get; set; }
        public List<EventoCotaViewModel> Cotas { get; set; }
        public EventoAgendaViewModel Agenda { get; set; }
        public List<EventoRecursoViewModel> Recursos { get; set; }
        public List<EventoHorarioViewModel> Horarios { get; set; }

        public EventoViewModel(long id)
        {
            Id = id;
        }
    }
}
