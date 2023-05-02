using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trilhas.Models.Usuario
{
	public class EventoFinalizadoViewModel
	{
		public long CursistaId { get; set; }
		public string Eixo { get; set; }
		public string Estacao { get; set; }
		public string Solucao { get; set; }
		public string Situacao { get; set; }
	}
}
