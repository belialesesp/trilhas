using System.ComponentModel.DataAnnotations.Schema;

namespace Trilhas.Data.Model.Cadastro
{
    public class PessoaContato : DefaultEntity
    {
		[ForeignKey("PessoaId")]
		public Pessoa Pessoa { get; set; }

		public string Numero { get; set; }

        [ForeignKey("TipoPessoaContatoId")]
        public TipoPessoaContato TipoPessoaContato { get; set; }
        public long TipoPessoaContatoId { get; set; }
    }
}
