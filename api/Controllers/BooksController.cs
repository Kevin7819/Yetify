using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using api.Mappers;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private const string BooksFilePath = "Books/books.json";
        private readonly IUserBookProgressService _progressService;

        public BooksController(IUserBookProgressService progressService)
        {
            _progressService = progressService;
        }

        // GET: api/books?userId=1
        [HttpGet]
        public async Task<IActionResult> GetAllBooks([FromQuery] int userId)
        {
            try
            {
                if (userId <= 0)
                    return BadRequest(new { error = "UserId es requerido y debe ser mayor a 0." });

                if (!System.IO.File.Exists(BooksFilePath))
                    return NotFound(new { error = "No se encontraron libros." });

                var jsonData = System.IO.File.ReadAllText(BooksFilePath);
                var books = JsonSerializer.Deserialize<List<Book>>(jsonData) ?? new List<Book>();

                // Obtener progreso para todos los libros
                var bookIds = books.Select(b => b.Id).ToList();
                var progresses = await _progressService.GetProgressForBooksAsync(userId, bookIds);

                // Mapear con progreso
                var simplifiedBooks = books.Select(b => b.toDto(progresses[b.Id])).ToList();

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

        // GET: api/books/1?userId=1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById([FromRoute] int id, [FromQuery] int userId)
        {
            try
            {
                if (userId <= 0)
                    return BadRequest(new { error = "UserId es requerido y debe ser mayor a 0." });

                if (!System.IO.File.Exists(BooksFilePath))
                    return NotFound(new { error = "No se encontraron libros." });

                var jsonData = System.IO.File.ReadAllText(BooksFilePath);
                var books = JsonSerializer.Deserialize<List<Book>>(jsonData) ?? new List<Book>();

                var book = books.FirstOrDefault(b => b.Id == id);
                if (book == null)
                    return NotFound(new { error = $"No se encontró un libro con ID {id}." });

                // Obtener o crear progreso
                var progress = await _progressService.GetProgressAsync(userId, id);
                
                // Si no existe, crear registro con progreso 0
                if (progress == 0.0)
                {
                    await _progressService.CreateOrUpdateProgressAsync(userId, id, 0.0);
                }

                return Ok(book.toContentDto(progress));
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

        // GET: api/books/search/harry?userId=1
        [HttpGet("search/{search}")]
        public async Task<IActionResult> GetSearchBooks([FromRoute] string search, [FromQuery] int userId)
        {
            try
            {
                if (userId <= 0)
                    return BadRequest(new { error = "UserId es requerido y debe ser mayor a 0." });

                if (!System.IO.File.Exists(BooksFilePath))
                    return NotFound(new { error = "No se encontraron libros." });

                var jsonData = System.IO.File.ReadAllText(BooksFilePath);
                var books = JsonSerializer.Deserialize<List<Book>>(jsonData) ?? new List<Book>();

                var searchLower = search.ToLower();

                var filteredBooks = books
                    .Where(b =>
                        (!string.IsNullOrEmpty(b.Title) && b.Title.ToLower().Contains(searchLower)) ||
                        (!string.IsNullOrEmpty(b.Author) && b.Author.ToLower().Contains(searchLower))
                    )
                    .ToList();

                // Obtener progreso para libros filtrados
                var bookIds = filteredBooks.Select(b => b.Id).ToList();
                var progresses = await _progressService.GetProgressForBooksAsync(userId, bookIds);

                var result = filteredBooks.Select(b => b.toDto(progresses[b.Id])).ToList();

                return Ok(result);
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

        // PUT: api/books/1/progress?userId=1
        [HttpPut("{id}/progress")]
        public async Task<IActionResult> UpdateProgress([FromRoute] int id, [FromQuery] int userId, [FromBody] UpdateProgressRequest request)
        {
            try
            {
                if (userId <= 0)
                    return BadRequest(new { error = "UserId es requerido y debe ser mayor a 0." });

                if (request.Progress < 0 || request.Progress > 1)
                    return BadRequest(new { error = "El progreso debe estar entre 0 y 1." });

                // Verificar que el libro existe
                if (!System.IO.File.Exists(BooksFilePath))
                    return NotFound(new { error = "No se encontraron libros." });

                var jsonData = System.IO.File.ReadAllText(BooksFilePath);
                var books = JsonSerializer.Deserialize<List<Book>>(jsonData) ?? new List<Book>();
                
                if (!books.Any(b => b.Id == id))
                    return NotFound(new { error = $"No se encontró un libro con ID {id}." });

                var progress = await _progressService.CreateOrUpdateProgressAsync(userId, id, request.Progress);

                return Ok(new { 
                    message = "Progreso actualizado exitosamente.",
                    progress = progress.Progress,
                    updatedAt = progress.UpdatedAt
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Ocurrió un error al actualizar el progreso.",
                    detail = ex.Message
                });
            }
        }
    }

    public class UpdateProgressRequest
    {
        [Range(0, 1)]
        public double Progress { get; set; }
    }
}