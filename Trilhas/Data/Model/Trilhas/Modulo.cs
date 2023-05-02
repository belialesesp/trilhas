using System.ComponentModel.DataAnnotations.Schema;

namespace Trilhas.Data.Model.Trilhas
{
    public class Modulo : Entity
    {
        [ForeignKey("CursoId")]
        public Curso Curso { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public int CargaHoraria { get; set; }
    }
}
