using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.UserTask;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/usertask")]
    [ApiController]
    public class UserTaskController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public UserTaskController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tasks = await _context.UserTasks.ToListAsync();
            var tasksDto = tasks.Select(task => task.ToDto());
            return Ok(tasksDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var task = await _context.UserTasks.FirstOrDefaultAsync(t => t.Id == id);
            if (task == null) return NotFound();
            return Ok(task.ToDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserTaskRequestDto taskDto)
        {
            var taskModel = taskDto.ToTaskFromCreateDto();
            await _context.UserTasks.AddAsync(taskModel);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = taskModel.Id }, taskModel.ToDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserTaskRequestDto taskDto)
        {
            var taskModel = await _context.UserTasks.FirstOrDefaultAsync(t => t.Id == id);
            if (taskModel == null) return NotFound();

            taskModel.Title = taskDto.Title;
            taskModel.Description = taskDto.Description;
            taskModel.IsCompleted = taskDto.IsCompleted;
            taskModel.DueDate = taskDto.DueDate;

            await _context.SaveChangesAsync();
            return Ok(taskModel.ToDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var taskModel = await _context.UserTasks.FirstOrDefaultAsync(t => t.Id == id);
            if (taskModel == null) return NotFound();

            _context.UserTasks.Remove(taskModel);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
