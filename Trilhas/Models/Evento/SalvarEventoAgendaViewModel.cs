using System;

namespace Trilhas.Models.Evento
{
    public class SalvarEventoAgendaViewModel
    {
        public SalvarEventoAgendaViewModel(long id)
        {
            Id = id;
        }
        public long Id { get; set; }
        public DateTime DataInscricaoInicio { get; set; }
        public DateTime HoraInscricaoInicio { get; set; }
        public DateTime DataInscricaoFim { get; set; }
        public DateTime HoraInscricaoFim { get; set; }
        public int NumeroVagas { get; set; }

        public string Justificativa { get; set; }
    }
}
