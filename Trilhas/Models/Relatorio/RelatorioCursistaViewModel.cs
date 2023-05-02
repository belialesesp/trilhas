using System;
using System.Collections.Generic;

namespace Trilhas.Models.Relatorio
{
    public class RelatorioCursistaViewModel
    {
        public RelatorioCursistaViewModel()
        {
            ListaInscritos = new List<RelatorioCursistaInscritosViewModel>();
        }
        public DateTime DataAtual { get; set; }
        public string Nome { get; set; }
        public string Entidade { get; set; }
        public string Local { get; set; }
        public string Periodo { get; set; }
        public string Horario { get; set; }
        public string Relatorio { get; set; }
		public List<RelatorioCursistaInscritosViewModel> ListaInscritos { get; set; }
    }
}
