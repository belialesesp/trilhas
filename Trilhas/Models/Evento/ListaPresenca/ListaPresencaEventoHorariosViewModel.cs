using System;

namespace Trilhas.Models.Evento.ListaPresenca
{
    public class ListaPresencaEventoHorariosViewModel
    {
        public long EventoHorarioId { get; set; }
        public string Modulo { get; set; }
        public DateTime DataHoraInicio { get; set; }
        public DateTime DataHoraFim { get; set; }
        public bool Selecionar { get; set; }
    }
}
