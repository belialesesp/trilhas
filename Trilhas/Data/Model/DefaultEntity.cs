using System;

namespace Trilhas.Data
{
    public class DefaultEntity : Entity
    {
        public DateTime CreationTime { get; set; }
        public string CreatorUserId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string DeletionUserId { get; set; }
    }
}
