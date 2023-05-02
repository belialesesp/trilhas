using System.ComponentModel.DataAnnotations.Schema;
using Trilhas.Data.Model.Cadastro;

namespace Trilhas.Data.Model.Eventos
{
    public class EventoCota : Entity
    {
        [ForeignKey("EventoId")]
        public Evento Evento { get; set; }

        [ForeignKey("EntidadeId")]
        public Entidade Entidade { get; set; }

        public int Quantidade { get; set; }
    }
}
