using api.Data;
using api.Dtos.UserTask;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using api.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        [HttpPost("user/{idUser}")]
        public async Task<IActionResult> CreateTaskByUser([FromRoute] int idUser, [FromBody] CreateUserTaskRequestDto taskDto)
        {
            // Validate that the user exists (optional)
            var userExists = await _context.Users.AnyAsync(u => u.Id == idUser);
            if (!userExists)
            {
            return NotFound(new 
            { 
                message = $"User with id {idUser} not found",
                suggestion = "Please provide a valid user ID"
            });
            }

            // Validate input data
            if (taskDto == null)
            {
            return BadRequest(new { message = MessageConstants.Generic.RequiredFields });
            }
            if (string.IsNullOrWhiteSpace(taskDto.description))
            {
            return BadRequest(new { message = MessageConstants.FieldRequired("La descripción de la tarea") });
            }
            if (taskDto.idCourse <= 0)
            {
            return BadRequest(new { message = MessageConstants.FieldRequired("El curso") });
            }

            // Set default status if not provided
            if (string.IsNullOrWhiteSpace(taskDto.status))
            {
            taskDto.status = UserTaskConstants.DefaultStatus;
            }

            // Create the task assigned to the specified user
            var taskModel = taskDto.ToTaskFromCreateDto();
            taskModel.idUser = idUser;  // Assign to the user from the route parameter

            await _context.UserTasks.AddAsync(taskModel);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(
            nameof(GetById), 
            new { id = taskModel.id }, 
            new { 
                message = $"Task created successfully for user {idUser}",
                task = taskModel.ToDto() 
            });
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
        [HttpGet("user/{idUser}")]
        public async Task<IActionResult> GetTasksByUserId([FromRoute] int idUser)
        {
            // Retrieve all tasks that belong to a specific user by their ID
            var tasks = await _context.UserTasks
                .Where(t => t.idUser == idUser)
                .ToListAsync();

            // If no tasks are found, return a 404 with a message
            if (!tasks.Any())
            {
                return NotFound(new { message = $"No tasks found for the user with id {idUser}" });
            }

            // Convert the list of task entities to DTOs
            var taskDtos = tasks.Select(t => t.ToDto());

            // Return the tasks as a successful response
            return Ok(taskDtos);
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