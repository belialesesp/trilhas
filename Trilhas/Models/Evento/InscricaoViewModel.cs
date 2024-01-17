using System;

namespace Trilhas.Models.Evento
{
    public class InscricaoViewModel
	{
		public long Id { get; set; }
		public CursistaViewModel Cursista { get; set; }
		public DateTime DataDeInscricao { get; set; }
        public string Situacao { get; set; } 
        public double Frequencia { get; set; }
        public PenalidadeViewModel Penalidade { get; set; }
        public bool Selecionar { get; set; }
    }
}
