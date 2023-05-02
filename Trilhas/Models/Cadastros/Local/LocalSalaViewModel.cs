namespace Trilhas.Models.Cadastros.Local
{
    public class LocalSalaViewModel
	{
		public long Id { get; set; }
		public string Sigla { get; set; }
		public string Numero { get; set; }
		public int Capacidade { get; set; }
        public bool Alocada { get; set; }
		public LocalSalaViewModel(long id){
			Id = id;
		}
	}
}
