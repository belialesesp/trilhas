using System.Collections.Generic;

namespace Trilhas.Models.Cadastros
{
    public class SalvarEntidadeViewModel
	{
		public long Id { get; set; }
		public string Nome { get; set; }
		public string Sigla { get; set; }
		public long TipoEntidadeId { get; set; }
		public long MunicipioId { get; set; }
		public List<long> Gestores { get; set; }
	}
}