namespace api.Dtos.Book
{
    public class BookRequestDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        
        public double Progress { get; set; } = 0.0;
    }
}