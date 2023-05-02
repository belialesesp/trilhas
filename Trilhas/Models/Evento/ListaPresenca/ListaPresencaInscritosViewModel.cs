namespace Trilhas.Models.Evento.ListaPresenca
{
    public class ListaPresencaInscritosViewModel
    {
        public long Id { get; set; }
        public long EventoHorarioId { get; set; }
        public long PessoaId { get; set; }
        public string PessoaNome { get; set; }
        public bool Presente { get; set; }
    }
}
