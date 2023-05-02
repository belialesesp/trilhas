using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Trilhas.Data.Model.Trilhas;

namespace Trilhas.Data.Model.Cadastro
{
	public class Habilitacao: Entity
	{
		[ForeignKey("DocenteId")]
		public Docente Docente { get; set; }
		[ForeignKey("CursoId")]
		public Curso Curso { get; set; }
	}
}
