using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

// Main user entity model
namespace api.Models
{
    // Custom ApplicationUser extending IdentityUser
    public class User : IdentityUser<int>
    {

        public string Role { get; set; }              // Optional: role info (can also use IdentityRole)
        public DateTime Birthday { get; set; }        // Custom field
        public DateTime RegistrationDate { get; set; } // Custom field

        // Navigation property for tasks
        public List<UserTask> UserTasks { get; set; } = new();
    }
}