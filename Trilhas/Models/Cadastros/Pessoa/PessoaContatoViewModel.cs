using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trilhas.Models.Cadastros.Local;

namespace Trilhas.Models.Cadastros.Pessoa
{
	public class PessoaContatoViewModel
	{
		public long? Id { get; set; }
		public string Nome { get; set; }
		public string Numero { get; set; }
		public string TipoContato { get; set; }
        public long TipoContatoId { get; set; }
        public TipoContatoViewModel Tipo { get; set; }

        public PessoaContatoViewModel(long id){
			Id = id;
		}
	}
}
