using System.ComponentModel.DataAnnotations.Schema;

namespace Trilhas.Data.Model.Trilhas
{
    public class Habilidade : Entity
    {
        [ForeignKey("CursoId")]
        public Curso Curso { get; set; }
        public string Descricao { get; set; }
    }
}
