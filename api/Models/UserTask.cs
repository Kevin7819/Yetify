using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    // User task model with relationships
    public class UserTask
    {
        [Key]
        public int id { get; set; }            // Task ID

        // User relationship
        [ForeignKey("User")]
        public int idUser { get; set; }
        public User User { get; set; }

        // Course relationship  
        [ForeignKey("Course")]
        public int idCourse { get; set; }
        public Course Course { get; set; }

        public string description { get; set; }  // Task details
        public DateTime dueDate { get; set; }    // Due date
        public string status { get; set; }       // Progress status
    }
}