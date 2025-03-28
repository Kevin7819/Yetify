namespace api.Dtos.UserTask
{
    public class CreateUserTaskRequestDto
    {
        public string title { get; set; }
        public string description { get; set; }
        public DateTime dueDate { get; set; }
    }
}
