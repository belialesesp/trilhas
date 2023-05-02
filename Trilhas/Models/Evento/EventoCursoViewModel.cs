using System.Collections.Generic;
using Trilhas.Data.Enums;
using Trilhas.Models.Trilhas.SolucaoEducacional;

namespace Trilhas.Models.Evento
{
    public class EventoCursoViewModel
    {
        public long Id { get; set; }
        public EnumModalidade ModalidadeDeCurso { get; set; }
        public string Titulo { get; set; }
        public bool PermiteCertificado { get; set; }
        public List<ModuloViewModel> Modulos { get; set; }
        public int CargaHorariaTotal { get; set; }

        public EventoCursoViewModel(long id)
        {
            Id = id;
        }
    }
}
