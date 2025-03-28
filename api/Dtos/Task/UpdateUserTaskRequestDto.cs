namespace api.Dtos.UserTask
{
    public class UpdateUserTaskRequestDto
    {
        public int idUser { get; set; }
        public int idCourse { get; set; }
        public string description { get; set; } 
        public DateTime dueDate { get; set; }
        public string status{ get; set; }
    }
}
