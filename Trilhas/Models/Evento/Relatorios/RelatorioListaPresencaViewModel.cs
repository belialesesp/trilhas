using System;
using System.Collections.Generic;

namespace Trilhas.Models.Evento.Relatorios
{
	public class RelatorioListaPresencaViewModel
	{
		public RelatorioListaPresencaViewModel()
		{
			Evento = new EventoListaPresencaManualViewModel();
			ListaInscritos = new List<InscritoListaPresencaManualViewModel>();
			Datas = new List<PeriodoListaPresencaViewModel>();
		}
		public EventoListaPresencaManualViewModel Evento { get; set; }
		public List<InscritoListaPresencaManualViewModel> ListaInscritos { get; set; }
		public DateTime DataInicio { get; set; }
		public DateTime DataFim { get; set; }
		public DateTime DataAtual { get; set; }
		public string Relatorio { get; set; }		
		public List<PeriodoListaPresencaViewModel> Datas { get; set; }
	}
}
