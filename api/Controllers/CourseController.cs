using api.Data;
using api.Dtos.Course;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using api.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public CourseController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _context.Courses.ToListAsync();

            if (!courses.Any())
            {
                return NotFound(new { message = MessageConstants.EntityNotFound("cursos") });
            }

            var coursesDto = courses.Select(course => course.ToDto());
            return Ok(coursesDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.id == id);
            if (course == null) 
            {
                return NotFound(new {
                    error = MessageConstants.EntityNotFound("El curso"),
                    suggestion = MessageConstants.Generic.TryAgain
                });
            }
            return Ok(course.ToDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCourseRequestDto courseDto)
        {   
            if (string.IsNullOrWhiteSpace(courseDto.nameCourse))
            {
                return BadRequest(new {
                    error = MessageConstants.FieldRequired("Nombre del curso"),
                    suggestion = "Por favor, ingrese un nombre válido para el curso."
                });
            }

            bool exists = await _context.Courses.AnyAsync(c => c.nameCourse.ToLower() == courseDto.nameCourse.ToLower());
    
            if (exists)
            {
                return Conflict(new {
                    error = $"El curso '{courseDto.nameCourse}' ya existe.",
                    suggestion = "Intente guardar el curso con un nombre diferente."
                });
            }

            var courseModel = courseDto.ToCourseFromCreateDto();
            await _context.Courses.AddAsync(courseModel);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetById), new { id = courseModel.id }, 
                new { 
                    message = MessageConstants.EntityCreated($"El curso '{courseModel.nameCourse}'"), 
                    course = courseModel.ToDto() 
                });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCourseRequestDto courseDto)
        {
            var courseModel = await _context.Courses.FirstOrDefaultAsync(c => c.id == id);
            if (courseModel == null) 
            {
                return NotFound(new {
                    error = MessageConstants.EntityNotFound("El curso"),
                    suggestion = MessageConstants.Generic.TryAgain
                });
            }

            if (string.IsNullOrWhiteSpace(courseDto.nameCourse))
            {
                return BadRequest(new {
                    error = MessageConstants.FieldRequired("Nombre del curso"),
                    suggestion = "Por favor, ingrese un nombre válido para el curso."
                });
            }

            if (courseModel.nameCourse.ToLower() == courseDto.nameCourse.ToLower())
            {
                courseModel.description = courseDto.description;
                await _context.SaveChangesAsync();
                return Ok(new { 
                    message = MessageConstants.EntityUpdated($"El curso '{courseModel.nameCourse}'"), 
                    course = courseModel.ToDto() 
                });
            }

            bool exists = await _context.Courses.AnyAsync(c => c.nameCourse.ToLower() == courseDto.nameCourse.ToLower() && c.id != id);
            if (exists)
            {
                return Conflict(new {
                    error = $"El curso '{courseDto.nameCourse}' ya existe.",
                    suggestion = "Intente actualizarlo con un nombre diferente."
                });
            }

            courseModel.nameCourse = courseDto.nameCourse;
            courseModel.description = courseDto.description;

            await _context.SaveChangesAsync();
            return Ok(new { 
                message = MessageConstants.EntityUpdated($"El curso '{courseModel.nameCourse}'"), 
                course = courseModel.ToDto() 
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var courseModel = await _context.Courses.FirstOrDefaultAsync(c => c.id == id);
            if (courseModel == null) 
            {
                return NotFound(new { 
                    error = MessageConstants.EntityNotFound("El curso"),
                    suggestion = "Intente eliminar uno diferente."
                });
            }

            _context.Courses.Remove(courseModel);
            await _context.SaveChangesAsync();
            return Ok(new { 
                message = MessageConstants.EntityDeleted($"El curso '{courseModel.nameCourse}'") 
            });
        }
    }
}