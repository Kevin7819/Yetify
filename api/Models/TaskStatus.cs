using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    // TaskStatus model with tasks relationship
    public class TaskStatus
    {
        [Key]
        public int id { get; set; } // Unique identifier for the task status

        public string status { get; set; } // Status description (e.g., "Pending", "Completed")

        // Associated tasks collection
        //public List<UserTask> UserTasks { get; set; } = new List<UserTask>();
    }
}