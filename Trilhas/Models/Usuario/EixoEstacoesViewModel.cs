using System.Collections.Generic;
using Trilhas.Models.Trilhas.Eixo;
using Trilhas.Models.Trilhas.Estacao;

namespace Trilhas.Models.Usuario
{
    public class EixoEstacoesViewModel
	{
		public EixoViewModel Eixo { get; set; }
		public List<EstacaoViewModel> Estacoes { get; set; }
	}
}
