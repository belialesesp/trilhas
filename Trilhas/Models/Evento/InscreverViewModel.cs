using System;

namespace Trilhas.Models.Evento
{
    public class InscreverViewModel
    {
        public long EventoId { get; set; }
        public long PessoaId { get; set; }
        public DateTime DataDeInscricao { get; set; }

        public InscreverViewModel()
        {
            DataDeInscricao = DateTime.Now;
        }
    }
}
