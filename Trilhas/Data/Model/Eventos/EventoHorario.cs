using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Data.Model.Trilhas;

namespace Trilhas.Data.Model.Eventos
{
    public class EventoHorario : Entity
    {
        [ForeignKey("EventoId")]
        public Evento Evento { get; set; }
        [ForeignKey("ModuloId")]
        public Modulo Modulo { get; set; }
        [ForeignKey("LocalSalaId")]
        public LocalSala Sala { get; set; }
        [ForeignKey("DocenteId")]
        public Docente Docente { get; set; }
        public FuncaoDocente Funcao { get; set; }
        public DateTime DataHoraInicio { get; set; }
        public DateTime DataHoraFim { get; set; }
        public List<RegistroDePresenca> ListaDePresenca { get; set; }

        public double TotalDeHoras()
        {
            return (DataHoraFim - DataHoraInicio).TotalHours;
        }

        public bool Presente(Pessoa cursista)
        {
            return ListaDePresenca != null ? ListaDePresenca.Any(x => x.Pessoa.Id == cursista.Id && x.Presente) : false;
        }
    }
}
