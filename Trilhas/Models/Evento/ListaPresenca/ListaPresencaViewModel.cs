using System.Collections.Generic;

namespace Trilhas.Models.Evento.ListaPresenca
{
    public class ListaPresencaViewModel
    {
        public List<ListaPresencaInscritosViewModel> Inscritos { get; set; }
        public List<ListaPresencaEventoHorariosViewModel> EventoHorarios { get; set; }
        public string EventoTitulo { get; set; }
        public long EventoId { get; set; }
    }
}
