using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class UserTask
    {
        [Key]
        public int id { get; set; }

        [ForeignKey("User")]
        public int idUser { get; set; }
        public User User { get; set; }  // Relación con User


       [ForeignKey("Course")]
        public int idCourse { get; set; }
        public Course Course { get; set; }  // Relación con Course
        
        public string description { get; set; } 
        public DateTime dueDate { get; set; }
        public string status{ get; set; }
    }
}
