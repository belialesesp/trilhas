using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trilhas.Models.Evento
{
	public class EtiquetaViewModel
	{
		public string Nome { get; set; }
		public string Entidade { get; set; }
		public long CursistaId { get; set; }
		public long EventoId { get; set; }
	}
}
