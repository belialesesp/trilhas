using System.ComponentModel.DataAnnotations.Schema;

namespace Trilhas.Data.Model.Cadastro
{
    public class Recurso: DefaultEntity
	{
		public string Nome { get; set; }
		public string Descricao { get; set; }
        [Column(TypeName = "decimal(7, 2)")]
        public decimal Custo { get; set; }
	}
}
