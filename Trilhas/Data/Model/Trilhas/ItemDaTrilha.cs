using System.ComponentModel.DataAnnotations.Schema;

namespace Trilhas.Data.Model.Trilhas
{
    public class ItemDaTrilha : DefaultEntity
    {
        [ForeignKey("TrilhaId")]
        public TrilhaDoUsuario Trilha { get; set; }
        [ForeignKey("SolucaoId")]
        public SolucaoEducacional SolucaoEducacional { get; set; }
    }
}
