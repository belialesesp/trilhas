using System.Collections.Generic;

namespace Trilhas.Models.Usuario
{
    public class VisualizarTrilhaViewModel
    {
        public Dictionary<string, Dictionary<string, List<AgrupadorSolucaoViewModel>>> AgrupadorEixos { get; set; }

        public VisualizarTrilhaViewModel()
        {
            AgrupadorEixos = new Dictionary<string, Dictionary<string, List<AgrupadorSolucaoViewModel>>>();
        }

        public void Adicionar(string eixo, string estacao, long solucaoId, string solucao, string tipoSolucao)
        {
            if (!AgrupadorEixos.ContainsKey(eixo))
            {
                List<AgrupadorSolucaoViewModel> agrupadorSolucoes = new List<AgrupadorSolucaoViewModel>();
                agrupadorSolucoes.Add(new AgrupadorSolucaoViewModel(solucaoId, solucao, tipoSolucao));

                Dictionary<string, List<AgrupadorSolucaoViewModel>> agrupadorEstacoes = new Dictionary<string, List<AgrupadorSolucaoViewModel>>();
                agrupadorEstacoes.Add(estacao, agrupadorSolucoes);

                AgrupadorEixos.Add(eixo, agrupadorEstacoes);
            }
            else
            {
                var agrupadorEstacoes = AgrupadorEixos[eixo];

                if (!agrupadorEstacoes.ContainsKey(estacao))
                {
                    List<AgrupadorSolucaoViewModel> agrupadorSolucoes = new List<AgrupadorSolucaoViewModel>();
                    agrupadorSolucoes.Add(new AgrupadorSolucaoViewModel(solucaoId, solucao, tipoSolucao));

                    agrupadorEstacoes.Add(estacao, agrupadorSolucoes);
                }
                else
                {
                    var agrupadorSolucoes = agrupadorEstacoes[estacao];
                    agrupadorSolucoes.Add(new AgrupadorSolucaoViewModel(solucaoId, solucao, tipoSolucao));
                }
            }
        }

		public long IdUsuario { get; set; }
		public string UsuarioCPF { get; set; }
		public string UsuarioEmail { get; set; }
	}

    public class AgrupadorSolucaoViewModel
    {
        public long SolucaoId { get; set; }
        public string SolucaoNome { get; set; }
        public string SolucaoTipo { get; set; }

        public AgrupadorSolucaoViewModel(long id, string nome, string tipo)
        {
            SolucaoId = id;
            SolucaoNome = nome;
            SolucaoTipo = tipo;
        }
    }
}
