using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trilhas.Data.Model;

namespace Trilhas.Models.Cadastros.Local
{
	public class GridLocalViewModel
	{
		public long Id { get; set; }
		public string Nome { get; set; }
		public string Logradouro { get; set; }
		public int QtdSalas { get; set; }
		public int CapacidadeTotal { get; set; }
		public List<GridLocalSalaViewModel> Salas { get; set; }
		public bool Excluido { get; set; }
	}
}
