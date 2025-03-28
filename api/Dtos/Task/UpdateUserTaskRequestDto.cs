namespace api.Dtos.UserTask
{
    public class UpdateUserTaskRequestDto
    {
        public string title { get; set; }
        public string description { get; set; }
        public bool isCompleted { get; set; }
        public DateTime dueDate { get; set; }
    }
}
