using System;
using System.ComponentModel.DataAnnotations.Schema;
using Trilhas.Data.Enums;
using Trilhas.Data.Model.Cadastro;

namespace Trilhas.Data.Model.Eventos
{
    public class Inscrito : DefaultEntity
    {
        [ForeignKey("ListaDeInscricaoId")]
        public ListaDeInscricao ListaDeInscricao { get; set; }
        [ForeignKey("PessoaId")]
        public Pessoa Cursista { get; set; }
        public DateTime DataDeInscricao { get; set; }
        public EnumSituacaoCursista Situacao { get; set; }
        public double Frequencia { get; set; }
        public Penalidade Penalidade { get; set; }
    }
}
