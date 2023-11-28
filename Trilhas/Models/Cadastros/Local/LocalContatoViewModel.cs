namespace Trilhas.Models.Cadastros.Local
{
    public class LocalContatoViewModel
	{
		public long Id { get; set; }
        public long TipoContatoId { get; set; }
        public TipoContatoViewModel Tipo { get; set; }
		public string Numero { get; set; }
		public LocalContatoViewModel(long id){
			Id = id;
		}
	}

    public class TipoContatoViewModel
    {
        public string Nome { get; set; }
    }
}
