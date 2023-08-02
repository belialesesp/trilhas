using System;

namespace Trilhas.Models.Evento
{
	public class GridEventoViewModel
    {
        public long Id { get; set; }
        public string Entidade { get; set; }
        public string Evento { get; set; }
        public string Situacao { get; set; }
        public string CargaHoraria { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string Municipio { get; set; }
        public string Docente { get; set; }
        public long ListaDeInscricaoId { get; set; }
        public int Inscritos { get; set; }
        public int Aprovados { get; set; }
        public int Declarados { get; set; }
        public int Desistentes { get; set; }
        public bool Ead { get; set; }
        public string Modalidade { get; set; }
    }
}
