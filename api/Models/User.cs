using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class User
    {
        [Key]
        public int id { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        public string email { get; set; }
        public DateTime birthday { get; set; }
        public DateTime registrationDate { get; set; }

         // Relación con UserTask
        public List<UserTask> UserTasks { get; set; }= new List<UserTask>();
    }
}
