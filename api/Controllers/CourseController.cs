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
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
            if (course == null) return NotFound();
            return Ok(course.ToDto());
        }
        //crear una nueva materia
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCourseRequestDto courseDto)
        {
            var courseModel = courseDto.ToCourseFromCreateDto();
            await _context.Courses.AddAsync(courseModel);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id= courseModel.Id }, courseModel.ToDto());
        }

        //actualizar materia
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCourseRequestDto courseDto)
        {
            var courseModel = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
            if (courseModel == null) return NotFound();

            courseModel.NameCourse = courseDto.NameCourse;
            courseModel.Description = courseDto.Description;

            await _context.SaveChangesAsync();
            return Ok(courseModel.ToDto());
        }

        // Eliminar una materia
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var courseModel = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
            if (courseModel == null) return NotFound();

            _context.Courses.Remove(courseModel);
            await _context.SaveChangesAsync();
            return NoContent();
        }
}
}