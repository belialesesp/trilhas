namespace Trilhas.Models.Evento
{
    public class EventoEntidadeViewModel
	{
		public long Id { get; set; }
		public string Nome { get; set; }

		public EventoEntidadeViewModel(long id)
		{
			Id = id;
		}
	}
}