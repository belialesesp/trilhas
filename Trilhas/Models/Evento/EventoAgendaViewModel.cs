using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trilhas.Models.Evento
{
	public class EventoAgendaViewModel
	{
		public EventoAgendaViewModel(long id)
		{
			Id = id;
		}
		public long Id { get; set; }
		public DateTime DataInicio { get; set; }
		public DateTime DataFim { get; set; }
		public DateTime DataInscricaoInicio { get; set; }
		public DateTime HoraInscricaoInicio { get; set; }
		public DateTime DataInscricaoFim { get; set; }
		public DateTime HoraInscricaoFim { get; set; }
		public int NumeroVagas { get; set; }

		public string Justificativa { get; set; }
	}
}
