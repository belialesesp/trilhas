namespace Trilhas.Models.Cadastros.Recurso
{
    public class RecursoViewModel
	{
		public long Id { get; set; }
		public string Nome { get; set; }
		public string Descricao { get; set; }
		public decimal Custo { get; set; }

		public RecursoViewModel(long id){
			Id = id;
		}
	}
}
