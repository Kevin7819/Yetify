using api.Data;
using api.Dtos.Book;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private const string BooksFilePath = "Books/books.json";

        // GET: api/books
        [HttpGet]
        public IActionResult GetAllBooks()
        {
            try
            {
                if (!System.IO.File.Exists(BooksFilePath))
                    return NotFound(new { error = "No se encontraron libros." });

                var jsonData = System.IO.File.ReadAllText(BooksFilePath);
                var books = JsonSerializer.Deserialize<List<Book>>(jsonData) ?? new List<Book>();

                // only ID, Title, Author
                var simplifiedBooks = books.Select(b => b.toDto()).ToList();

                return Ok(simplifiedBooks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Ocurrió un error al obtener los libros.",
                    detail = ex.Message
                });
            }
        }

        // GET: api/books/{id}
        [HttpGet("{id}")]
        public IActionResult GetBookById([FromRoute] int id)
        {
            try
            {
                if (!System.IO.File.Exists(BooksFilePath))
                    return NotFound(new { error = "No se encontraron libros." });

                var jsonData = System.IO.File.ReadAllText(BooksFilePath);
                var books = JsonSerializer.Deserialize<List<Book>>(jsonData) ?? new List<Book>();

                var book = books.FirstOrDefault(b => b.Id == id);
                if (book == null)
                    return NotFound(new { error = $"No se encontró un libro con ID {id}." });

                // all data book: ID, Title, Author, Content
                return Ok(book.toContentDto());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = $"Ocurrió un error al obtener el libro con ID {id}.",
                    detail = ex.Message
                });
            }
        }
    }
}
