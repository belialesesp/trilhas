using System;
using System.Collections.Generic;
using Trilhas.Models.Cadastros.Docente;

namespace Trilhas.Models.Evento
{
    public class EventoFinalizadoViewModel
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Entidade { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public DateTime DataInicioInscricao { get; set; }
        public DateTime DataFimInscricao { get; set; }
        public bool PossuiCertificado { get; set; }
        public List<InscricaoViewModel> Inscritos { get; set; }
        public int TotalInscritos { get; set; }
        public int TotalDeclarados { get; set; }
        public int TotalCertificados { get; set; }
        public int TotalReprovados { get; set; }
        public List<DocenteViewModel> Docentes { get; set; }
        public EventoFinalizadoViewModel()
        {
            Inscritos = new List<InscricaoViewModel>();
            Docentes = new List<DocenteViewModel>();
        }
    }
}
