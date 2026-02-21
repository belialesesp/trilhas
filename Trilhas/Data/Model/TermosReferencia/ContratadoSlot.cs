using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trilhas.Data.Model.TermosReferencia
{
    /// <summary>
    /// Represents an individual hiring slot for a professional
    /// Each slot can be filled with a contractor's name and marked as verified
    /// </summary>
    public class ContratadoSlot
    {
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Reference to the parent item
        /// </summary>
        public long TermoReferenciaItemId { get; set; }
        
        [ForeignKey("TermoReferenciaItemId")]
        public virtual TermoReferenciaItem Item { get; set; }

        /// <summary>
        /// Slot number (1, 2, 3, etc. for the same professional type)
        /// </summary>
        public int NumeroSlot { get; set; }

        /// <summary>
        /// Name of the contracted professional (filled by user)
        /// </summary>
        [MaxLength(200)]
        public string NomeContratado { get; set; }

        /// <summary>
        /// Checkbox to attest/verify the contractor
        /// </summary>
        public bool Ateste { get; set; }

        /// <summary>
        /// When this slot was filled
        /// </summary>
        public DateTime? DataContratacao { get; set; }

        /// <summary>
        /// Who filled this slot
        /// </summary>
        [MaxLength(450)]
        public string PreenchidoPorUserId { get; set; }

        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }
}