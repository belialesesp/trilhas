using System.ComponentModel.DataAnnotations.Schema;

namespace Trilhas.Data.Model.Trilhas
{
    public class SolucaoEducacional : DefaultEntity
    {
        [ForeignKey("EstacaoId")]
        public Estacao Estacao { get; set; }
        public string Titulo { get; set; }
        public string TipoDeSolucao { get; set; }
    }
}
