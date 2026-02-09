using System.ComponentModel.DataAnnotations;

namespace Trilhas.Models.TermosReferencia
{
    public class TermoReferenciaViewModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(500, ErrorMessage = "O título deve ter no máximo 500 caracteres")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "O ano é obrigatório")]
        [Range(2020, 2050, ErrorMessage = "Ano inválido")]
        public int Ano { get; set; }

        [StringLength(100)]
        public string NumeroDocumento { get; set; }

        [StringLength(2000)]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O status é obrigatório")]
        public string Status { get; set; }

        public string DataAprovacao { get; set; }
    }

    public class TermoReferenciaItemViewModel
    {
        public long Id { get; set; }

        public long TermoDeReferenciaId { get; set; }

        [Required(ErrorMessage = "O curso é obrigatório")]
        [StringLength(200)]
        public string Curso { get; set; }

        [Required(ErrorMessage = "O tipo de profissional é obrigatório")]
        [StringLength(100)]
        public string Profissional { get; set; }

        [Range(0, 9999, ErrorMessage = "Quantidade inválida")]
        public int Quantidade { get; set; }

        [Range(0, 9999.99, ErrorMessage = "Carga horária inválida")]
        public decimal CargaHoraria { get; set; }

        [StringLength(50)]
        public string MesExecucao { get; set; }

        public string DataOferta { get; set; }

        [StringLength(50)]
        public string Modalidade { get; set; }

        [Range(0, 9999, ErrorMessage = "Quantidade de turmas inválida")]
        public int QuantidadeTurmas { get; set; }

        [Range(0, 9999, ErrorMessage = "Quantidade de alunos inválida")]
        public int AlunosPorTurma { get; set; }

        [Range(0, 999999.99, ErrorMessage = "Valor por hora inválido")]
        public decimal ValorHora { get; set; }

        [Range(0, 100, ErrorMessage = "Percentual de encargos deve estar entre 0 e 100")]
        public decimal EncargosPercentual { get; set; }

        public decimal ValorTotal { get; set; }
    }
}
