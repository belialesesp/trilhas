using System;
using System.ComponentModel.DataAnnotations.Schema;
using Trilhas.Data.Model.Cadastro;

namespace Trilhas.Data.Model.Eventos
{
    public class Penalidade : Entity
    {
        [ForeignKey("PessoaId")]
        public Pessoa Cursista { get; set; }
        public DateTime DataDaPenalidade { get; set; }
        public DateTime DataInicioPenalidade { get; set; }
        public DateTime DataFimPenalidade { get; set; }
        public string JustificativaDeCancelamento { get; set; }

        public Penalidade()
        {
        }

        public Penalidade(Pessoa cursista, DateTime inicio, int diasDePenalidade)
        {
            Cursista = cursista;
            DataInicioPenalidade = inicio;
            DataFimPenalidade = DataInicioPenalidade.AddDays(diasDePenalidade);
            DataDaPenalidade = DateTime.Now;
        }

        public bool Cancelada()
        {
            return !string.IsNullOrEmpty(JustificativaDeCancelamento);
        }
    }
}
