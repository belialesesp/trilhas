using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trilhas.Data.Model;

namespace Trilhas.Models.Trilhas.Estacao
{
	public class EstacaoViewModel
	{
		public long Id { get; set; }
		//public Eixo Eixo { get; set; }
		public long EixoId { get; set; }
		public string Nome { get; set; }
		public string Descricao { get; set; }
		public bool Excluido { get; set; }

		public EstacaoViewModel(long id)
		{
			Id = id;
		}
		public bool Selecionado{ get; set; }
	}
}
