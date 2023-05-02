using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trilhas.Models.Cadastros.Recurso
{
	public class SalvarRecursoViewModel
	{
		public long Id { get; set; }
		public string Nome { get; set; }
		public string Descricao { get; set; }
		public decimal Custo { get; set; }
	}
}
