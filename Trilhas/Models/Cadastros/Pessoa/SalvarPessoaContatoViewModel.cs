using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trilhas.Models.Cadastros.Pessoa
{
	public class SalvarPessoaContatoViewModel
	{
		public long Id { get; set; }
		public string Numero { get; set; }
		public string TipoContato { get; set; }
	}
}
