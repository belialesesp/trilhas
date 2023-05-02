using System;

namespace Trilhas.Models.Evento
{
    public class EventoHorarioViewModel
	{
        public long Id { get; set; }
        //public ModuloViewModel Modulo { get; set; }
        public long ModuloId { get; set; }
        public string ModuloNome { get; set; }
        public long DocenteId { get; set; }
        public string DocenteNome { get; set; }
        public long FuncaoDocenteId { get; set; }
        public string FuncaoDocenteNome { get; set; }
        public long SalaId { get; set; }
        public EventoSalaViewModel Sala { get; set; }
		public DateTime DataInicio { get; set; }
		public DateTime HoraInicio { get; set; }
		public DateTime DataFim { get; set; }
		public DateTime HoraFim { get; set; }

        public EventoHorarioViewModel(long id)
        {
            Id = id;
        }
    }
}
