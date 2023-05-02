using System;

namespace Trilhas.Models.Evento
{
    public class EventoListaInscricaoViewModel
	{
		public long Id { get; set; }
		public string Nome { get; set; }
		public string Entidade { get; set; }
		public DateTime DataInicio { get; set; }
		public DateTime DataFim { get; set; }
		public DateTime DataInicioInscricao { get; set; }
		public DateTime DataFimInscricao { get; set; }
	}
}
