using System;

namespace Trilhas.Data
{
    public class NotModifierEntity : Entity
    {
        public DateTime CreationTime { get; set; }
        public string CreatorUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string DeletionUserId { get; set; }
    }
}
