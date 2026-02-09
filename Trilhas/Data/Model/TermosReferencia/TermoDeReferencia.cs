using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trilhas.Data.Model.TermosReferencia
{
    /// <summary>
    /// Represents a Termo de Referência (Terms of Reference) document
    /// </summary>
    [Table("TermosDeReferencia")]
    public class TermoDeReferencia
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Titulo { get; set; }

        [Required]
        public int Ano { get; set; }

        [StringLength(100)]
        public string NumeroDocumento { get; set; }

        [StringLength(2000)]
        public string Descricao { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime? DataAprovacao { get; set; }

        [StringLength(50)]
        public string Status { get; set; } // "Rascunho", "Aprovado", "Em Execução", "Concluído"

        // Storage path for the original uploaded document
        [StringLength(500)]
        public string CaminhoArquivoOriginal { get; set; }

        // Audit fields
        public string CreatorUserId { get; set; }
        public DateTime CreationTime { get; set; }
        public string LastModifierUserId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string DeletionUserId { get; set; }
        public DateTime? DeletionTime { get; set; }

        // Navigation property
        public virtual ICollection<TermoReferenciaItem> Itens { get; set; }

        public TermoDeReferencia()
        {
            Itens = new HashSet<TermoReferenciaItem>();
        }
    }

    /// <summary>
    /// Represents each row in the planning table (Anexo II)
    /// </summary>
    [Table("TermoReferenciaItens")]
    public class TermoReferenciaItem
    {
        [Key]
        public long Id { get; set; }

        public long TermoDeReferenciaId { get; set; }

        [Required]
        [StringLength(200)]
        public string Curso { get; set; }

        [Required]
        [StringLength(100)]
        public string Profissional { get; set; } // Docente, Conteudista, Assistente

        public int Quantidade { get; set; }

        public decimal CargaHoraria { get; set; }

        [StringLength(50)]
        public string MesExecucao { get; set; }

        public DateTime? DataOferta { get; set; }

        [StringLength(50)]
        public string Modalidade { get; set; } // Presencial, EAD, Híbrido

        public int QuantidadeTurmas { get; set; }

        public int AlunosPorTurma { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorHora { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal EncargosPercentual { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorTotal { get; set; }

        // Audit fields
        public string CreatorUserId { get; set; }
        public DateTime CreationTime { get; set; }
        public string LastModifierUserId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string DeletionUserId { get; set; }
        public DateTime? DeletionTime { get; set; }

        // Navigation property
        [ForeignKey("TermoDeReferenciaId")]
        public virtual TermoDeReferencia TermoDeReferencia { get; set; }
    }
}
