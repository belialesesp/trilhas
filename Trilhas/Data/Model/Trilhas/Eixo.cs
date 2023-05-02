using System.ComponentModel.DataAnnotations.Schema;

namespace Trilhas.Data.Model.Trilhas
{
    public class Eixo : DefaultEntity
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        [NotMapped]
        public string Imagem { get; set; }
    }
}
