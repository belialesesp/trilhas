using System.ComponentModel.DataAnnotations.Schema;
using Trilhas.Data.Model.Cadastro;

namespace Trilhas.Data.Model.Eventos
{
    public class EventoRecurso : Entity
    {
        [ForeignKey("EventoId")]
        public Evento Evento { get; set; }
        [ForeignKey("RecursoId")]
        public Recurso Recurso { get; set; }
        public int Quantidade { get; set; }
    }
}
