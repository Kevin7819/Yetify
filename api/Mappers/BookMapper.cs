using api.Dtos.Book;
using api.Models;

namespace api.Mappers
{
    public static class BookMappers
    {
        public static BookRequestDto toDto(this Book book)
        {
            return new BookRequestDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author
            };
        }

        public static BookContentDto toContentDto(this Book book)
        {
            return new BookContentDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Content = book.Content
            };
        }
    }
}