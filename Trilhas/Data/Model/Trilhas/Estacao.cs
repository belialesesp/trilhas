using System.ComponentModel.DataAnnotations.Schema;

namespace Trilhas.Data.Model.Trilhas
{
    public class Estacao : DefaultEntity
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        [ForeignKey("EixoId")]
        public Eixo Eixo { get; set; }
    }
}
