using System.Text.Json.Serialization;

namespace api.Models
{
        public class Book
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }
            
            [JsonPropertyName("title")]
            public string Title { get; set; } = string.Empty;
            
            [JsonPropertyName("author")]
            public string Author { get; set; } = string.Empty;
            
            [JsonPropertyName("content")]
            public string Content { get; set; } = string.Empty;
        }
}