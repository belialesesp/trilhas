namespace Trilhas.Models.Cadastros.Pessoa
{
    public class GridPessoaViewModel
	{
		public long Id { get; set; }
		public string Nome { get; set; }
		public string Cpf { get; set; }
		public string NumeroFuncional { get; set; }
		public string NomeSocial { get; set; }
		public string Endereco { get; set; }
		public string Entidade { get; set; }
		public string Email { get; set; }
		public bool Excluido { get; set; }
	}
}
