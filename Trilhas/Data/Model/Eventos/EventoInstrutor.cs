//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;


//namespace Trilhas.Data.Model
//{
//    public class EventoInstrutor : Entity
//    {
//		[ForeignKey("EventoId")]
//		public Evento Evento { get; set; }

//		[ForeignKey("InstrutorPessoaId")]
//        public Pessoa Instrutor { get; set; }

//        [ForeignKey("SolucaoEducacionalId")]
//        public SolucaoEducacional SolucaoEducacional { get; set; }
//        [ForeignKey("ModuloId")]
//        public Modulo Modulo { get; set; }

//        [ForeignKey("FuncaoId")]
//        public Funcao Funcao { get; set; }

//        public DateTime DataHoraInicio { get; set; }
//        public DateTime DataHoraFim { get; set; }
//    }

//}
