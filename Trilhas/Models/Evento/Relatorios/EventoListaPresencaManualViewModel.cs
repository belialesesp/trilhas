using System;

namespace Trilhas.Models.Evento.Relatorios
{
    public class EventoListaPresencaManualViewModel
	{
		public string EventoNome { get; set; }
		public string EventoSigla { get; set; }
		public string EntidadeNome { get; set; }
		public long LocalId { get; set; }
		public string LocalNome { get; set; }
		public DateTime DataInicio { get; set; }
		public DateTime DataFim { get; set; }
		public DateTime HoraInicio { get; set; }
		public DateTime HoraFim { get; set; }
	}
}
