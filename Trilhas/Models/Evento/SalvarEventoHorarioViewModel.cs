using System;

namespace Trilhas.Models.Evento
{
    public class SalvarEventoHorarioViewModel
    {
        public long Id { get; set; }
        public long ModuloId { get; set; }
        public long DocenteId { get; set; }
        public long SalaId { get; set; }
        public long FuncaoDocenteId { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime HoraInicio { get; set; }
        public DateTime DataFim { get; set; }
        public DateTime HoraFim { get; set; }
    }
}
