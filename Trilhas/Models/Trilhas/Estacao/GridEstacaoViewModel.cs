using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trilhas.Models.Trilhas.Eixo;

namespace Trilhas.Models.Trilhas.Estacao
{
	public class GridEstacaoViewModel
	{
		public long Id { get; set; }
		public string EixoNome { get; set; }
		public string Nome { get; set; }
		public string Descricao { get; set; }
		public bool Excluido { get; set; }
	}
}
