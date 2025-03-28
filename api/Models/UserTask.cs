namespace api.Models
{
    public class UserTask
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public bool isCompleted { get; set; }
        public DateTime dueDate { get; set; }
    }
}
