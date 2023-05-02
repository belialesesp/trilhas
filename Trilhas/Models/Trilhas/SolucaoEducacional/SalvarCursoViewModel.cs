using System.Collections.Generic;
using Trilhas.Data.Enums;

namespace Trilhas.Models.Trilhas.SolucaoEducacional
{
    public class SalvarCursoViewModel
    {
        public long Id { get; set; }
        public long EstacaoId { get; set; }
        public string Titulo { get; set; }
        public string Sigla { get; set; }
        public string Descricao { get; set; }
		public long TipoCursoId { get; set; }
        public EnumModalidade Modalidade { get; set; }
        public long NivelCursoId { get; set; }
        public bool PermiteCertificado { get; set; }
        public int FrequenciaMinimaCertificado { get; set; }
        public int FrequenciaMinimaDeclaracao { get; set; }
        public string PreRequisitos { get; set; }
        public string PublicoAlvo { get; set; }
        public string ConteudoProgramatico { get; set; }
        public List<HabilidadeViewModel> Habilidades { get; set; }
        public List<ModuloViewModel> Modulos { get; set; }
    }
}
