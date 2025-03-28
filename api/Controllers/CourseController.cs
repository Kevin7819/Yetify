using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Course;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers{
    
    [Authorize]
    [Route("api/course")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public CourseController(ApplicationDBContext context)
        {
            _context = context;
        }

        // get all courses
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _context.Courses.ToListAsync();

            if (!courses.Any()) // list is empty
            {
            return NotFound(new { message = "No hay cursos registrados." });
            }

            var coursesDto = courses.Select(course => course.ToDto());
            return Ok(coursesDto);
        }
    

        //get a course by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.id == id);
            if (course == null) 
            {
                return NotFound(new{
                     error = "El curso no fue encontrado." ,
                     sugerencia = "Intente nuevamente." 
                });
             }
            return Ok(course.ToDto());
        }


        // Create a new course
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCourseRequestDto courseDto)
        {   
            // Validate that the name is not empty
            if (string.IsNullOrWhiteSpace(courseDto.nameCourse))
            {
                return BadRequest(new
            {
                error = "El nombre del curso no puede estar vacío.",
                sugerencia = "Por favor, ingrese un nombre válido para el curso."
                });
        }

            // Check if a course with the same name already exists
            bool exists = await _context.Courses.AnyAsync(c => c.nameCourse.ToLower() == courseDto.nameCourse.ToLower());
    
            if (exists)
            {
                return BadRequest(new {
                     error = $"El curso '{courseDto.nameCourse}' ya existe.",
                     sugerencia = "Intente guardar el curso con un nombre diferente."
                });
            }

            var courseModel = courseDto.ToCourseFromCreateDto();
            await _context.Courses.AddAsync(courseModel);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id= courseModel.id }, 
                new { message = $"El curso '{courseModel.nameCourse}' se ha guardado correctamente.", course = courseModel.ToDto() });
        }

        // Update an existing course
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCourseRequestDto courseDto)
        {
            var courseModel = await _context.Courses.FirstOrDefaultAsync(c => c.id == id);
            if (courseModel == null) 
            {
                return NotFound(new 
                {
                    error = "El curso no fue encontrado.",
                    sugerencia = "Intente nuevamente." 
                });
            }

              // Validate that the name is not empty
            if (string.IsNullOrWhiteSpace(courseDto.nameCourse))
            {
                return BadRequest(new
                {
                    error = "El nombre del curso no puede estar vacío.",
                    sugerencia = "Por favor, ingrese un nombre válido para el curso."
                });
            }


           // If the course name remains the same, only update the description
           if (courseModel.nameCourse.ToLower() == courseDto.nameCourse.ToLower())
           {
           courseModel.description = courseDto.description;
           await _context.SaveChangesAsync();
           return Ok(new { message = $"El curso '{courseModel.nameCourse}' se ha actualizado correctamente.", course = courseModel.ToDto() });
           }

            // If the name is changed, check that no other course has the same name
            bool exists = await _context.Courses.AnyAsync(c => c.nameCourse.ToLower() == courseDto.nameCourse.ToLower() && c.id != id);
            if (exists)
            {
                return BadRequest(new {
                     error = $"El curso '{courseDto.nameCourse}' ya existe.",
                     sugerencia = "Intente actualizarlo con un nombre diferente."});
            }

            // Update name and description if no conflict exists
            courseModel.nameCourse = courseDto.nameCourse;
            courseModel.description = courseDto.description;

            await _context.SaveChangesAsync();
            return Ok(new { message = $"El curso '{courseModel.nameCourse}' se ha actualizado correctamente.", course = courseModel.ToDto() });
        }

        // Delete a course
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var courseModel = await _context.Courses.FirstOrDefaultAsync(c => c.id == id);
            if (courseModel == null) 
            {
                return NotFound(new { 
                    error = "El curso no fue encontrado.",
                    sugerencia = "Intente eliminar uno diferente."
                });
            }


            _context.Courses.Remove(courseModel);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"El curso '{courseModel.nameCourse}' se ha eliminado correctamente." });
        }
}
}