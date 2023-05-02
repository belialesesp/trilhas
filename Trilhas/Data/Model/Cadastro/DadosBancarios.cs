using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Trilhas.Data.Model.Cadastro
{
	public class DadosBancarios: Entity
	{
		[ForeignKey("DocenteId")]
		public Docente Docente { get; set; }
		public string Banco { get; set; }
		public string ContaCorrente { get; set; }
		public string Agencia { get; set; }
	}
}
