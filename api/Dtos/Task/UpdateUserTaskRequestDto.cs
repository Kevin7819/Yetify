namespace api.Dtos.UserTask
{
    // DTO for updating user tasks
    public class UpdateUserTaskRequestDto
    {
        public int idUser { get; set; }         // User ID (required)
        public int idCourse { get; set; }       // Course ID (required)
        public string description { get; set; }  // Task details
        public DateTime dueDate { get; set; }    // New deadline
        public string status { get; set; }      // Current status
    }
}