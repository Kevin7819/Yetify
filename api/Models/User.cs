using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    // Main user entity model
    public class User
    {
        [Key]
        public int id { get; set; }                  // Primary key
        
        public string userName { get; set; }         // Unique username
        public string password { get; set; }         // Hashed password
        public string role { get; set; }             // User role/privileges
        public string email { get; set; }            // Unique email
        public DateTime birthday { get; set; }       // Date of birth
        public DateTime registrationDate { get; set; } // Account creation date

        // Navigation property for user's tasks
        public List<UserTask> UserTasks { get; set; } = new List<UserTask>();
    }
}