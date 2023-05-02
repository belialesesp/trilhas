using System.ComponentModel.DataAnnotations.Schema;
using Trilhas.Data.Model.Cadastro;

namespace Trilhas.Data.Model.Eventos
{
    public class RegistroDePresenca : Entity
    {
        [ForeignKey("EventoHorarioId")]
        public virtual EventoHorario EventoHorario { get; set; }
        [ForeignKey("PessoaId")]
        public virtual Pessoa Pessoa { get; set; }
        public bool Presente { get; set; }
    }
}
