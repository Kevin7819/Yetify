using api.Data;
using api.Dtos.UserTask;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using api.Constants;

namespace api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
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
                return NotFound(new { message = MessageConstants.Generic.NoRecords });
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
                    error = MessageConstants.EntityNotFound("La tarea"),
                    suggestion = MessageConstants.Generic.TryAgain
                });
            } 
            return Ok(task.ToDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserTaskRequestDto taskDto)
        {   
            // Get authenticated user ID
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = MessageConstants.Generic.Unauthorized });
            }

            int userId = int.Parse(userIdClaim);

            // Validate the input data
            if (taskDto == null)
            {
                return BadRequest(new { message = MessageConstants.Generic.RequiredFields });
            }
            if (string.IsNullOrWhiteSpace(taskDto.description))
            {
                return BadRequest(new { message = MessageConstants.FieldRequired("La descripción de la tarea") });
            }
            if (taskDto.status == null)
            {
                return BadRequest(new { message = MessageConstants.FieldRequired("El estado de la tarea") });
            }
            if (taskDto.idCourse == 0)
            {
                return BadRequest(new { message = MessageConstants.FieldRequired("El curso") });
            }
            
            // Set default status if not provided
            if (string.IsNullOrWhiteSpace(taskDto.status))
            {
                taskDto.status = UserTaskConstants.DefaultStatus;
            }

            // Convert DTO to model and save to database
            var taskModel = taskDto.ToTaskFromCreateDto();
            taskModel.idUser = userId;  // assign user

            await _context.UserTasks.AddAsync(taskModel);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetById), new { id = taskModel.id }, 
                new { 
                    message = MessageConstants.EntityCreated("La tarea"), 
                    task = taskModel.ToDto() 
                });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserTaskRequestDto taskDto)
        {
            // Check if the task exists before validating input
            var taskModel = await _context.UserTasks.FirstOrDefaultAsync(t => t.id == id);
            if (taskModel == null)
            {
                return NotFound(new { 
                    message = MessageConstants.EntityNotFound("La tarea"),
                    suggestion = MessageConstants.Generic.TryAgain
                });
            }

            // Validate the input data
            if (taskDto == null)
            {
                return BadRequest(new { message = MessageConstants.Generic.RequiredFields });
            }
            if (string.IsNullOrWhiteSpace(taskDto.description))
            {
                return BadRequest(new { message = MessageConstants.FieldRequired("La descripción de la tarea") });
            }
            if (taskDto.status == null)
            {
                return BadRequest(new { message = MessageConstants.FieldRequired("El estado de la tarea") });
            }
            if (taskDto.idUser == 0)
            {
                return BadRequest(new { message = MessageConstants.FieldRequired("El usuario") });
            }
            if (taskDto.idCourse == 0)
            {
                return BadRequest(new { message = MessageConstants.FieldRequired("El curso") });
            }
    
            // Update task data
            taskModel.idUser = taskDto.idUser;
            taskModel.idCourse = taskDto.idCourse;
            taskModel.description = taskDto.description; 
            taskModel.dueDate = taskDto.dueDate;
            taskModel.status = taskDto.status;

            await _context.SaveChangesAsync();
            
            return Ok(new { 
                message = MessageConstants.EntityUpdated("La tarea"), 
                task = taskModel.ToDto() 
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            // Check if the task exists
            var taskModel = await _context.UserTasks.FirstOrDefaultAsync(t => t.id == id);
            if (taskModel == null) 
            {
                return NotFound(new { 
                    error = MessageConstants.EntityNotFound("La tarea"),
                    suggestion = "Intente eliminar una diferente."
                });
            }
            
            // Remove task from database
            _context.UserTasks.Remove(taskModel);
            await _context.SaveChangesAsync();
            
            return Ok(new { 
                message = MessageConstants.EntityDeleted("La tarea") 
            });
        }
    }
}