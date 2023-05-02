using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trilhas.Data.Model;

namespace Trilhas.Models.Cadastros.Local
{
	public class LocalViewModel
	{
		public long Id { get; set; }
		public string Nome { get; set; }
		public string Observacoes { get; set; }

		public string Cep { get; set; }
		public string Logradouro { get; set; }
		public string Bairro { get; set; }
		public string Numero { get; set; }
		public string Complemento { get; set; }
		public long MunicipioId { get; set; }
		public string Uf { get; set; }

		public List<LocalContatoViewModel> Contatos { get; set; }
		public List<LocalRecursoViewModel> Recursos { get; set; }
		public List<LocalSalaViewModel> Salas { get; set; }

		public LocalViewModel(long id)
		{
			Id = id;
		}
	}
}
