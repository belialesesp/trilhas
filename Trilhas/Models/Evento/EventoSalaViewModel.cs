namespace Trilhas.Models.Evento
{
    public class EventoSalaViewModel
    {
        public long Id { get; set; }
        public string Sigla { get; set; }
        public string Numero { get; set; }
        public int Capacidade { get; set; }

        public EventoSalaViewModel(long id)
        {
            Id = id;
        }
    }
}
