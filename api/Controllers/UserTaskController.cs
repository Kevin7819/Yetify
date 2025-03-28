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
            if (!tasks.Any())
            {
                return NotFound(new { message = "No hay tareas registradas." });
            }
            var tasksDto = tasks.Select(task => task.ToDto());
            return Ok(tasksDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var task = await _context.UserTasks.FirstOrDefaultAsync(t => t.id == id);
            if (task == null)
            {
                return NotFound(new
                {
                    error = "La tarea no fue encontrada.",
                    sugerencia = "Intente nuevamente."
                });

            } 
            return Ok(task.ToDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserTaskRequestDto taskDto)
        {
             // Validate the input data
             if (taskDto == null)
            {
                return BadRequest(new { message = "Los datos de la tarea son requeridos." });
            }
            if (string.IsNullOrWhiteSpace(taskDto.description))
            {
                return BadRequest(new { message = "La descripción de la tarea es requerida." });
            }
            if (taskDto.status == null)
            {
                return BadRequest(new { message = "El estado de la tarea es requerido." });
            }
            if (taskDto.idUser == 0)
            {
                return BadRequest(new { message = "El usuario es requerido." });
            }
            if (taskDto.idCourse == 0)
            {
                return BadRequest(new { message = "El curso es requerido." });
            }
            // Set default status if not provided
            if (string.IsNullOrWhiteSpace(taskDto.status))
            {
                taskDto.status = "Pendiente";
            }

           // Convert DTO to model and save to database
            var taskModel = taskDto.ToTaskFromCreateDto();
            await _context.UserTasks.AddAsync(taskModel);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id= taskModel.id }, 
                new { message = $"La tarea se ha guardado correctamente.", task = taskModel.ToDto() });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserTaskRequestDto taskDto)
        {


             // Check if the task exists before validating input
            var taskModel = await _context.UserTasks.FirstOrDefaultAsync(t => t.id == id);

            if (taskModel == null)
            {
                return NotFound(new { message = "La tarea no existe." });
            }

             // Validate the input data
            if (taskDto == null)
            {
                return BadRequest(new { message = "Los datos de la tarea son requeridos." });
            }
            if (string.IsNullOrWhiteSpace(taskDto.description))
            {
                return BadRequest(new { message = "La descripción de la tarea es requerida." });
            }
            if (taskDto.status == null)
            {
                return BadRequest(new { message = "El estado de la tarea es requerido." });
            }
            if (taskDto.idUser == 0)
            {
                return BadRequest(new { message = "El usuario es requerido." });
            }
            if (taskDto.idCourse == 0)
            {
            return BadRequest(new { message = "El curso es requerido." });
            }
    
            // Update task data
            taskModel.idUser = taskDto.idUser;
            taskModel.idCourse = taskDto.idCourse;
            taskModel.description = taskDto.description; 
            taskModel.dueDate = taskDto.dueDate;
            taskModel.status = taskDto.status;

            await _context.SaveChangesAsync();
            return Ok(new { message = $"La tarea se ha actualizado correctamente.", task = taskModel.ToDto() });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
             // Check if the task exists
            var taskModel = await _context.UserTasks.FirstOrDefaultAsync(t => t.id == id);
            if (taskModel == null) 
            {
                return NotFound(new { 
                    error = "La tarea no fue encontrado.",
                    sugerencia = "Intente eliminar una diferente."
                });
            }
            
              // Remove task from database
            _context.UserTasks.Remove(taskModel);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"La tarea se ha eliminado correctamente." });
        }
    }
}
