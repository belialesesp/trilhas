using System.Collections.Generic;

namespace Trilhas.Models.Trilhas.SolucaoEducacional
{
    public class GridSolucaoEducacionalViewModel
	{
		public long Id { get; set; }
        public string EixoNome { get; set; }
        public string EstacaoNome { get; set; }
		public string Titulo { get; set; }
		public string TipoDeSolucao { get; set; }
        public string ModalidadeDeCurso { get; set; }
        public bool PermiteCertificado { get; set; }
        public List<ModuloViewModel> Modulos { get; set; }
        public int CargaHorariaTotal { get; set; }
        public bool Excluido { get; set; }		
	}
}
