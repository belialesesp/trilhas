namespace Trilhas.Models.Evento
{
    public class EventoLocalViewModel
	{
		public long Id { get; set; }
		public string Nome { get; set; }
        public int CapacidadeTotal { get; set; }

        public EventoLocalViewModel(long id)
		{
			Id = id;
		}
	}
}