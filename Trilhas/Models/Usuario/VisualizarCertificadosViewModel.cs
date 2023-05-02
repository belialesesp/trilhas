using System.Collections.Generic;

namespace Trilhas.Models.Usuario
{
    public class VisualizarCertificadosViewModel
    {
        public VisualizarCertificadosViewModel()
        {
            EventosFinalizados = new List<EventoFinalizadoViewModel>();
        }
        public long IdUsuario { get; set; }
        public string UsuarioEmail { get; set; }
        public string UsuarioCPF { get; set; }
        public List<EventoFinalizadoViewModel> EventosFinalizados { get; set; }
    }
}
