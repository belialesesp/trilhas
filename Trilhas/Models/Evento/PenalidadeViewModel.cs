using System;

namespace Trilhas.Models.Evento
{
    public class PenalidadeViewModel
    {
        public long Id { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string Justificativa { get; set; }
        public bool Cancelada { get; set; }
        public DateTime DataDaPenalidade { get; set; }
    }
}
