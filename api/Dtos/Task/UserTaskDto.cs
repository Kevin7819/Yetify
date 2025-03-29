namespace api.Dtos.UserTask
{
    // Task response DTO
    public class UserTaskDto
    {
        public int id { get; set; }         // Unique identifier
        public int idUser { get; set; }      // Owner user ID
        public int idCourse { get; set; }   // Related course ID
        public string description { get; set; }  // Task content
        public DateTime dueDate { get; set; }   // Due date
        public string status { get; set; }  // Completion status
    }
}