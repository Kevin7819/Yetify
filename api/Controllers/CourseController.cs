using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Course;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
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
            var coursesDto = courses.Select(course => course.ToDto());
            return Ok(coursesDto);
        }
    

        //obtener una materia por id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.id == id);
            if (course == null) return NotFound();
            return Ok(course.ToDto());
        }


        // Create a new course
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCourseRequestDto courseDto)
        {
            // Check if a course with the same name already exists
            bool exists = await _context.Courses.AnyAsync(c => c.nameCourse.ToLower() == courseDto.nameCourse.ToLower());
    
            if (exists)
            {
                return BadRequest(new {
                     error = $"The course '{courseDto.nameCourse}' already exists.",
                     suggestion = "Try saving the course with a different name."  
                });
            }

            var courseModel = courseDto.ToCourseFromCreateDto();
            await _context.Courses.AddAsync(courseModel);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id= courseModel.id }, 
                new { message = $"The course '{courseModel.nameCourse}' was successfully inserted.", course = courseModel.ToDto() });
        }

        // Update an existing course
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCourseRequestDto courseDto)
        {
            var courseModel = await _context.Courses.FirstOrDefaultAsync(c => c.id == id);
            if (courseModel == null) 
            {
                return NotFound(new { error = $"The course was not found." });
            }

           // If the course name remains the same, only update the description
           if (courseModel.nameCourse.ToLower() == courseDto.nameCourse.ToLower())
           {
           courseModel.description = courseDto.description;
           await _context.SaveChangesAsync();
           return Ok(new { message = $"The course '{courseModel.nameCourse}' was successfully updated.", course = courseModel.ToDto() });
           }

            // If the name is changed, check that no other course has the same name
            bool exists = await _context.Courses.AnyAsync(c => c.nameCourse.ToLower() == courseDto.nameCourse.ToLower() && c.id != id);
            if (exists)
            {
                return BadRequest(new {
                     error = $"The course '{courseDto.nameCourse}' already exists.",
                     suggestion = "Try updating with a different name."});
            }

            // Update name and description if no conflict exists
            courseModel.nameCourse = courseDto.nameCourse;
            courseModel.description = courseDto.description;

            await _context.SaveChangesAsync();
            return Ok(new { message = $"The course '{courseModel.nameCourse}' was successfully updated.", course = courseModel.ToDto() });
        }

        // Delete a course
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var courseModel = await _context.Courses.FirstOrDefaultAsync(c => c.id == id);
            if (courseModel == null) 
            {
                return NotFound(new { 
                    error = "The course was not found.",
                    suggestion = "Try deleting a different course."
                });
            }


            _context.Courses.Remove(courseModel);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"The course '{courseModel.nameCourse}' was successfully deleted." });
        }
}
}