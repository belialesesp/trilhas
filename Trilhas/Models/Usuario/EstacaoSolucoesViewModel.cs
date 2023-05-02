using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trilhas.Models.Trilhas.Estacao;
using Trilhas.Models.Trilhas.SolucaoEducacional;

namespace Trilhas.Models.Usuario
{
	public class EstacaoSolucoesViewModel
	{
		public string EixoNome { get; set; }
		public string EstacaoNome { get; set; }
		public long EstacaoId { get; set; }
        public string EixoImagem { get; set; }

		public List<SolucaoEducacionalOpcaoViewModel> Solucoes { get; set; }
	}
}
