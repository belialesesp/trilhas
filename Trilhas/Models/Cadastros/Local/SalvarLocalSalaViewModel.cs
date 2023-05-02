using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trilhas.Models.Cadastros.Local
{
	public class SalvarLocalSalaViewModel
	{
		public long? Id { get; set; }
		public string Sigla { get; set; }
		public string Numero { get; set; }
		public int Capacidade { get; set; }
	}
}
