using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trilhas.Data.Model.Notifications
{
    /// <summary>
    /// Represents an in-app notification for users
    /// </summary>
    [Table("Notifications")]
    public class Notification
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(450)]
        public string UserId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(1000)]
        public string Message { get; set; }

        [StringLength(50)]
        public string Type { get; set; } // ALERT, INFO, SUCCESS, WARNING

        [Column(TypeName = "nvarchar(max)")]
        public string Data { get; set; } // JSON data

        public bool Read { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}