using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

// Main user entity model
namespace api.Models
{
    // Custom ApplicationUser extending IdentityUser
    public class User : IdentityUser<int>
    {
        public DateTime Birthday { get; set; }        // Custom field
        public DateTime RegistrationDate { get; set; } // Custom field
        
        public string? PasswordResetCode { get; set; }
        public DateTime? PasswordResetCodeExpires { get; set; }


        // Navigation property for tasks
        public List<UserTask> UserTasks { get; set; } = new();
    }
}