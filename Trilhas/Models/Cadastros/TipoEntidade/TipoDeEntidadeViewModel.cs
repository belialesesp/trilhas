namespace Trilhas.Models.Cadastros.TipoEntidade
{
    public class TipoDeEntidadeViewModel
	{
		public TipoDeEntidadeViewModel(long id)
		{
			Id = id;
		}
		public long Id { get; set; }
		public string Descricao { get; set; }

	}
}
