namespace api.Dtos.UserTask
{
    public class CreateUserTaskRequestDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
    }
}
