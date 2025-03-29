using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    // Course model with tasks relationship
    public class Course
    {
        [Key]
        public int id { get; set; }             // Unique identifier
        
        public string nameCourse { get; set; }  // Course name
        public string description { get; set; } // Course description
        
        // Associated tasks collection
        public List<UserTask> UserTasks { get; set; } = new List<UserTask>();
    }
}