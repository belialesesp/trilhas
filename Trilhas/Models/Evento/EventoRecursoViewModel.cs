namespace Trilhas.Models.Evento
{
    public class EventoRecursoViewModel
    {
        public long Id { get; set; }
        public long RecursoId { get; set; }
        public string Nome { get; set; }
        public decimal Custo { get; set; }
        public int Quantidade { get; set; }

        public EventoRecursoViewModel(long id)
        {
            Id = id;
        }
    }
}
