using System;
using System.ComponentModel.DataAnnotations;

namespace Trilhas.Models.TermosReferencia
{
    public class TermoReferenciaViewModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(500)]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "O ano é obrigatório")]
        public int Ano { get; set; }

        [StringLength(100)]
        public string NumeroDocumento { get; set; }

        [StringLength(2000)]
        public string Descricao { get; set; }

        [StringLength(200)]
        public string Demandante { get; set; }

        [StringLength(50)]
        public string DataInicio { get; set; }

        [StringLength(50)]
        public string DataTermino { get; set; }

        public int Duracao { get; set; }

        [Required(ErrorMessage = "O status é obrigatório")]
        [StringLength(50)]
        public string Status { get; set; }
    }

    public class TermoReferenciaItemViewModel
    {
        public long Id { get; set; }

        public long TermoDeReferenciaId { get; set; }

        [Required(ErrorMessage = "O curso é obrigatório")]
        [StringLength(200)]
        public string Curso { get; set; }

        [Required(ErrorMessage = "O profissional é obrigatório")]
        [StringLength(100)]
        public string Profissional { get; set; }

        [Required(ErrorMessage = "A quantidade é obrigatória")]
        [Range(1, 100, ErrorMessage = "A quantidade deve estar entre 1 e 100")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "A carga horária é obrigatória")]
        public decimal CargaHoraria { get; set; }

        [StringLength(50)]
        public string MesExecucao { get; set; }

        public string DataOferta { get; set; }

        [StringLength(50)]
        public string Modalidade { get; set; }

        public int QuantidadeTurmas { get; set; }

        public int AlunosPorTurma { get; set; }

        public decimal ValorHora { get; set; }

        public decimal EncargosPercentual { get; set; }

        public decimal ValorTotal { get; set; }

        public int Contratados { get; set; }
    }
}