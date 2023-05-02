using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trilhas.Models.Cadastros.Local
{
	public class SalvarLocalRecursoViewModel
	{
		public long? Id { get; set; }
		public long RecursoId { get; set; }
		public int Quantidade { get; set; }
	}
}
