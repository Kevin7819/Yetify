using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class UserBookProgress
    {
        [Key]
        public int Id { get; set; }
        
        public int UserId { get; set; }
        
        public int BookId { get; set; }
        
        [Range(0, 1)]
        public double Progress { get; set; } = 0.0;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}