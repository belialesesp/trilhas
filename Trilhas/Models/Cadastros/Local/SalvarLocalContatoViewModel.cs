using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trilhas.Models.Cadastros.Local
{
	public class SalvarLocalContatoViewModel
	{
        public long TipoContatoId { get; set; }
        public long? Id { get; set; }
		//public string Tipo { get; set; }
		public string Numero { get; set; }
	}
}
