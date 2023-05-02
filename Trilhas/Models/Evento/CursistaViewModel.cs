namespace Trilhas.Models.Evento
{
    public class CursistaViewModel
	{
		public long Id { get; set; }
		public string Nome { get; set; }
		public string Cpf { get; set; }
		public string NumeroFuncional { get; set; }
		public string Email { get; set; }
        public bool Inscrito { get; set; }
        public string Entidade { get; set; }
    }
}
