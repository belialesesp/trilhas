using System.ComponentModel.DataAnnotations.Schema;

namespace Trilhas.Data.Model.Cadastro
{
    public class LocalRecurso : DefaultEntity
    {
        [ForeignKey("LocalId")]
        public Local Local { get; set; }

        [ForeignKey("RecursoId")]
        public Recurso Recurso { get; set; }

        public int Quantidade { get; set; }
    }
}
