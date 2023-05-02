namespace Trilhas.Models.Cadastros
{
    public class GridEntidadeViewModel
	{
		public long Id { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; }
		public string Municipio { get; set; }
        public bool Excluido { get; set; }
    }
}
