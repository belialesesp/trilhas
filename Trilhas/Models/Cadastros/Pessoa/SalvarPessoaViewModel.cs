using System;
using System.Collections.Generic;

namespace Trilhas.Models.Cadastros.Pessoa
{
    public class SalvarPessoaViewModel
	{
		public long Id { get; set; }
		public string Imagem { get; set; }
		public string Nome { get; set; }
		public long Sexo { get; set; }
		public string Cpf { get; set; }
		public string NumeroFuncional { get; set; }
		public string NomeSocial { get; set; }
		public DateTime DataNascimento { get; set; }
		public long? NumeroIdentidade { get; set; }
		public string UfIdentidade { get; set; }
		public string Email { get; set; }

		public string NumeroTitulo { get; set; }
		public string Pis { get; set; }

		public bool FlagDeficiente { get; set; }
		public long OrgaoExpedidorIdentidade { get; set; }
		public long? Deficiencia { get; set; }
		public long Escolaridade { get; set; }
		public long EntidadeId { get; set; }
		public long Cidade { get; set; }

		public string Cep { get; set; }
		public string Logradouro { get; set; }
		public string Bairro { get; set; }
		public string Numero { get; set; }
		public string Complemento { get; set; }
		public string Uf { get; set; }
		
		public List<PessoaContatoViewModel> Contatos { get; set; }
	}
}
