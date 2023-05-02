using System.Collections.Generic;

namespace Trilhas.Models.Evento
{
    public class SalvarEventoViewModel
    {
        public long Id { get; set; }
        public string Observacoes { get; set; }
        public string UrlEad { get; set; }
        public string Situacao { get; set; }
        public bool LimitarVagas { get; set; }
        public int VagasPorEntidade { get; set; }
        public int? CertificadoId { get; set; }
        public int DeclaracaoCursistaId { get; set; }
        public int DeclaracaoDocenteId { get; set; }
        public EventoCoordenadorViewModel Coordenador { get; set; }
        public EventoCursoViewModel Curso { get; set; }
        public EventoEntidadeViewModel Entidade { get; set; }
        public EventoLocalViewModel Local { get; set; }

        public SalvarEventoAgendaViewModel Agenda { get; set; }
        public List<SalvarEventoHorarioViewModel> Horarios { get; set; }
        public List<SalvarEventoRecursoViewModel> Recursos { get; set; }
        public List<SalvarEventoCotaViewModel> Cotas { get; set; }
    }
}
