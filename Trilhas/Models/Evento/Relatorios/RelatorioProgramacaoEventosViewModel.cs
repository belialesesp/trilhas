using System.Collections.Generic;

namespace Trilhas.Models.Evento.Relatorios
{
    public class RelatorioProgramacaoEventosViewModel
    {
        public RelatorioProgramacaoEventosViewModel()
        {
            ListaPresencial = new List<ProgramacaoEventoCursoViewModel>();
            ListaEaD = new List<ProgramacaoEventoCursoViewModel>();
            ListaSemiPresencial = new List<ProgramacaoEventoCursoViewModel>();
        }
        public List<ProgramacaoEventoCursoViewModel> ListaPresencial { get; set; }
        public List<ProgramacaoEventoCursoViewModel> ListaEaD { get; set; }
        public List<ProgramacaoEventoCursoViewModel> ListaSemiPresencial { get; set; }
        public string Mes { get; set; }
        public int TotalVagas { get; set; }
    }
}
