using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    // Role model with users relationship
    public class Role
    {
        [Key]
        public int idRole { get; set; } // Unique identifier for the role

        public string name { get; set; } // Name of the role

        // Associated users collection
        //public List<User> Users { get; set; } = new List<User>();
    }
}