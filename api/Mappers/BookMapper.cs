using api.Dtos.Book;
using api.Models;

namespace api.Mappers
{
    public static class BookMappers
    {
        public static BookRequestDto toDto(this Book book, double progress = 0.0)
        {
            return new BookRequestDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Progress = progress
            };
        }

        public static BookContentDto toContentDto(this Book book, double progress = 0.0)
        {
            return new BookContentDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Content = book.Content,
                Progress = progress
            };
        }
    }
}