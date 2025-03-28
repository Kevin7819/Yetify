using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.Models 
{
    public class Course
    {
        [Key]
        public int id {get; set;}

        public string nameCourse {get; set;}

        public string description {get; set;}

         public List<UserTask> UserTasks { get; set; } = new List<UserTask>(); // Relación con UserTask


    }

}