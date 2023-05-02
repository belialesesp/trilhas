using System.Collections.Generic;
using Trilhas.Data.Enums;

namespace Trilhas.Data.Model.Trilhas
{
    public class Curso : SolucaoEducacional
    {
        public string Sigla { get; set; }
        public string Descricao { get; set; }
        public TipoDeCurso TipoDoCurso { get; set; }
        public EnumModalidade Modalidade { get; set; }
        public NivelDeCurso NivelDoCurso { get; set; }
        public bool PermiteCertificado { get; set; }
        public int FrequenciaMinimaCertificado { get; set; }
        public int FrequenciaMinimaDeclaracao { get; set; }
        public string PreRequisitos { get; set; }
        public string PublicoAlvo { get; set; }
        public string ConteudoProgramatico { get; set; }
        public List<Habilidade> Habilidades { get; set; }
        public List<Modulo> Modulos { get; set; }

        public Curso()
        {
            Habilidades = new List<Habilidade>();
            Modulos = new List<Modulo>();
        }

        public int CargaHorariaTotal()
        {
            int total = 0;

            foreach (var modulo in Modulos)
            {
                total += modulo.CargaHoraria;
            }

            return total;
        }
    }
}
