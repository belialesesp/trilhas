using System.ComponentModel.DataAnnotations.Schema;

namespace Trilhas.Data.Model.Cadastro
{
    public class LocalContato : DefaultEntity
    {
        [ForeignKey("LocalId")]
        public Local Local { get; set; }

        [ForeignKey("TipoContatoId")]
        public TipoLocalContato TipoContato { get; set; }

        public string NumeroTelefone { get; set; }
    }
}
