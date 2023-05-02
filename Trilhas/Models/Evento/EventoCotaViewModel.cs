namespace Trilhas.Models.Evento
{
    public class EventoCotaViewModel
	{
		public long Id { get; set; }
		public long EntidadeId { get; set; }
        public string EntidadeNome { get; set; }
        public int Quantidade { get; set; }

        public EventoCotaViewModel(long id)
        {
            Id = id;
        }
    }
}
