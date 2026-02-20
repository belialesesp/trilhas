using System;
using System.ComponentModel.DataAnnotations;

namespace Trilhas.Data.Model.Users
{
    public class UserProfile
    {
        [Key]
        public string UserId { get; set; }
        
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        
        public DateTime FirstLogin { get; set; }
        public DateTime LastLogin { get; set; }
        
        public bool ReceiveNotifications { get; set; } = true;
    }
}