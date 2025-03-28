namespace api.Dtos.UserTask
{
    public class UserTaskDto
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public bool isCompleted { get; set; }
        public DateTime dueDate { get; set; }
    }
}
