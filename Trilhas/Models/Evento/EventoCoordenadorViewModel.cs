namespace Trilhas.Models.Evento
{
    public class EventoGEDTHViewModel
	{
		public EventoGEDTHViewModel(long id) {
			Id = id;
		}

		public long Id { get; set; }
		public string Nome { get; set; }
	}
}
