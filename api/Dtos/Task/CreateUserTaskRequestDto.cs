namespace api.Dtos.UserTask
{
    // DTO for creating new user tasks
    public class CreateUserTaskRequestDto
    {
        public int idUser { get; set; }      // Required user ID
        public int idCourse { get; set; }     // Required course ID
        public string description { get; set; } // Task details/content
        public DateTime dueDate { get; set; }  // Deadline date
        public string status { get; set; }     // Initial status (e.g., "Pending")
    }
}