using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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

        /// <summary>
        /// Órgão/Entidade demandante (CEPDEC, SETOP, etc.)
        /// </summary>
        [StringLength(200)]
        public string Demandante { get; set; }

        /// <summary>
        /// Data de início do projeto (ex: "FEVEREIRO/2026")
        /// </summary>
        [StringLength(50)]
        public string DataInicio { get; set; }

        /// <summary>
        /// Data de término do projeto (ex: "NOVEMBRO/2026")
        /// </summary>
        [StringLength(50)]
        public string DataTermino { get; set; }

        /// <summary>
        /// Duração total em meses
        /// </summary>
        public int Duracao { get; set; }

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
    /// Represents each professional requirement row from the planning table (Anexo II)
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
        public string Profissional { get; set; } // DOCENTE, MODERADOR, DOCENTE_CONTEUDISTA

        public int Quantidade { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CargaHoraria { get; set; }

        [StringLength(50)]
        public string MesExecucao { get; set; }

        /// <summary>
        /// Data de oferta do curso (primeira data de oferta)
        /// </summary>
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

        /// <summary>
        /// Number of professionals already hired for this course/category
        /// </summary>
        public int Contratados { get; set; }

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
        public virtual ICollection<ContratadoSlot> Slots { get; set; } = new List<ContratadoSlot>();

/// <summary>
/// Helper property: Count of filled slots
/// </summary>
[NotMapped]
public int ContratadosCount 
{ 
    get 
    { 
        return Slots?.Where(s => !string.IsNullOrWhiteSpace(s.NomeContratado)).Count() ?? 0;
    } 
}

[NotMapped]
public int AtesteCount 
{ 
    get 
    { 
        return Slots?.Where(s => s.Ateste).Count() ?? 0;
    } 
}
    }
}