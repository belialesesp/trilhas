using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trilhas.Data.Model.Eventos
{
    public class EventoAgenda : DefaultEntity
    {
        // Agenda do Evento
        [ForeignKey("EventoId")]
        public Evento Evento { get; set; }

        public DateTime DataHoraInicio { get; set; }
        public DateTime DataHoraFim { get; set; }
        public DateTime DataHoraInscricaoInicio { get; set; }
        public DateTime DataHoraInscricaoFim { get; set; }
        public int NumeroVagas { get; set; }

        public string Justificativa { get; set; }
    }
}
