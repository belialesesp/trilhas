using System;

namespace Trilhas.Models.Cadastros.Docente
{
    public class DocenteFormacaoViewModel
	{
		public long Id { get; set; }
		public string Curso { get; set; }
		public string Titulacao { get; set; }
		public string Instituicao { get; set; }
		public int CargaHoraria { get; set; }
		public DateTime DataInicio { get; set; }
		public DateTime DataFim { get; set; }
	}
}
