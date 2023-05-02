using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trilhas.Models.Cadastros.Local
{
	public class LocalRecursoViewModel
	{
		public long Id { get; set; }
        public long RecursoId { get; set; }
        public string Nome { get; set; }
		public string Descricao { get; set; }
		public int Quantidade { get; set; }
		public LocalRecursoViewModel(long id){
			Id = id;
		}
	}
}
