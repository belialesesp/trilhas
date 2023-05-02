using System.ComponentModel.DataAnnotations.Schema;

namespace Trilhas.Data.Model.Cadastro
{
    public class LocalSala : DefaultEntity
    {
        [ForeignKey("LocalId")]
        public Local Local { get; set; }
        public string Sigla { get; set; }
        public string Numero { get; set; }
        public int Capacidade { get; set; }
    }
}
