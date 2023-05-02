using System.Collections.Generic;

namespace Trilhas.Models.Evento
{
    public class ListaInscritosViewModel
    {
        public EventoListaInscricaoViewModel Evento { get; set; }
        public List<InscricaoViewModel> Inscritos { get; set; }
        public ListaInscritosViewModel()
        {
            Evento = new EventoListaInscricaoViewModel();
            Inscritos = new List<InscricaoViewModel>();
        }
    }
}
