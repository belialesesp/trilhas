using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Trilhas.Data.Model.Cadastro
{
	public class Formacao: Entity
	{
		[ForeignKey("DocenteId")]
		public Docente Docente { get; set; }
		public string Curso { get; set; }
		public string Titulacao { get; set; }
		public string Instituicao { get; set; }
		public int CargaHoraria { get; set; }
		public DateTime DataInicio { get; set; }
		public DateTime DataFim { get; set; }
	}
}
