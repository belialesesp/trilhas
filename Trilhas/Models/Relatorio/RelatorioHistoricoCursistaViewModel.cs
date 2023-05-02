using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trilhas.Models.Relatorio
{
	public class RelatorioHistoricoCursistaViewModel
	{
		public RelatorioHistoricoCursistaViewModel()
		{
			ListaEventos = new List<RelatorioHistoricoCursistaEventoViewModel>();
		}
		public DateTime DataAtual { get; set; }
		public string NumeroFuncional { get; set; }
		public string Nome { get; set; }
		public string Email { get; set; }
		public string Entidade { get; set; }
		public string Endereco { get; set; }
		public string Telefone { get; set; }
		public string Relatorio { get; set; }
		public List<RelatorioHistoricoCursistaEventoViewModel> ListaEventos { get; set; }
	}
}
