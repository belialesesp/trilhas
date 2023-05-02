using System.Collections.Generic;

namespace Trilhas.Models.Evento
{
	public class GridEventoCompletaViewModel
	{
		public GridEventoCompletaViewModel(){
			ListaEventos = new List<GridEventoViewModel>();
		}
		public List<GridEventoViewModel> ListaEventos { get; set; }
		public int TotalInscritos { get; set; }
		public int TotalDeclarados { get; set; }
		public int TotalAprovados { get; set; }
		public int TotalDesistentes { get; set; }
		public int TotalCargaHoraria { get; set; }
	}
}