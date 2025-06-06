using api.Data;
using api.Dtos.TaskStatus;
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
    public class TaskStatusController : ControllerBase
    {
        // private readonly ApplicationDBContext _context;

        // public TaskStatusController(ApplicationDBContext context)
        // {
        //     _context = context;
        // }

        // [HttpGet]
        // public async Task<IActionResult> GetAll()
        // {
        //     var taskStatuses = await _context.TaskStatuses.ToListAsync();

        //     if (!taskStatuses.Any())
        //     {
        //         return NotFound(new { message = MessageConstants.EntityNotFound("tarea estado") });
        //     }

        //     var taskStatusesDto = taskStatuses.Select(ts => ts.ToDto());
        //     return Ok(taskStatusesDto);
        // }

        // [HttpGet("{id}")]
        // public async Task<IActionResult> GetById([FromRoute] int id)
        // {
        //     var taskStatus = await _context.TaskStatuses.FirstOrDefaultAsync(ts => ts.id == id);
        //     if (taskStatus == null)
        //     {
        //         return NotFound(new
        //         {
        //             error = MessageConstants.EntityNotFound("El estado de la tarea"),
        //             suggestion = MessageConstants.Generic.TryAgain
        //         });
        //     }
        //     return Ok(taskStatus.ToDto());
        // }

        // [HttpPost]
        // public async Task<IActionResult> Create([FromBody] CreateTaskStatusRequestDto taskStatusDto)
        // {
        //     if (string.IsNullOrWhiteSpace(taskStatusDto.status))
        //     {
        //         return BadRequest(new
        //         {
        //             error = MessageConstants.FieldRequired("Estado de la tarea"),
        //             suggestion = "Por favor, ingrese un estado válido para la tarea."
        //         });
        //     }

        //     bool exists = await _context.TaskStatuses.AnyAsync(ts => ts.status.ToLower() == taskStatusDto.status.ToLower());

        //     if (exists)
        //     {
        //         return Conflict(new
        //         {
        //             error = $"El estado de la tarea '{taskStatusDto.status}' ya existe.",
        //             suggestion = "Por favor, elija un estado diferente o actualice el existente."

        //         });
        //     }

        //     var taskStatusModel = taskStatusDto.ToTaskStatusFromCreateDto();
        //     await _context.TaskStatuses.AddAsync(taskStatusModel);
        //     await _context.SaveChangesAsync();

        //     return CreatedAtAction(nameof(GetById), new { id = taskStatusModel.id },
        //      new
        //      {
        //          message = MessageConstants.EntityCreated($"El estado de la tarea '{taskStatusModel.status}'"),
        //          taskStatus = taskStatusModel.ToDto()
        //      });
        // }

        // [HttpPut("{id}")]
        // public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CreateTaskStatusRequestDto taskStatusDto)
        // {
        //     var taskStatusModel = await _context.TaskStatuses.FirstOrDefaultAsync(ts => ts.id == id);
        //     if (taskStatusModel == null)
        //     {
        //         return NotFound(new
        //         {
        //             error = MessageConstants.EntityNotFound("El estado de la tarea"),
        //             suggestion = MessageConstants.Generic.TryAgain
        //         });
        //     }

        //     if (string.IsNullOrWhiteSpace(taskStatusDto.status))
        //     {
        //         return BadRequest(new
        //         {
        //             error = MessageConstants.FieldRequired("Estado de la tarea"),
        //             suggestion = "Por favor, ingrese un estado válido para la tarea."
        //         });
        //     }

        //     if (taskStatusModel.status.ToLower() == taskStatusDto.status.ToLower())
        //     {
        //         await _context.SaveChangesAsync();
        //         return Ok(new
        //         {
        //             message = MessageConstants.EntityUpdated($"El estado de la tarea '{taskStatusModel.status}'"),
        //             taskStatus = taskStatusModel.ToDto()
        //         });
        //     }
        //     bool exists = await _context.TaskStatuses.AnyAsync(ts => ts.status.ToLower() == taskStatusDto.status.ToLower() && ts.id != id);
        //     if (exists)
        //     {
        //         return Conflict(new
        //         {
        //             error = $"El estado de la tarea '{taskStatusDto.status}' ya existe.",
        //             suggestion = "Por favor, elija un estado diferente o actualice el existente."
        //         });
        //     }

        //     taskStatusModel.status = taskStatusDto.status;

        //     await _context.SaveChangesAsync();
        //     return Ok(new
        //     {
        //         message = MessageConstants.EntityUpdated($"El estado de la tarea '{taskStatusModel.status}'"),
        //         taskStatus = taskStatusModel.ToDto()
        //     });
        // }

        // [HttpDelete("{id}")]
        // public async Task<IActionResult> Delete([FromRoute] int id)
        // {
        //     var taskStatusModel = await _context.TaskStatuses.FirstOrDefaultAsync(ts => ts.id == id);
        //     if (taskStatusModel == null)
        //     {
        //         return NotFound(new
        //         {
        //             error = MessageConstants.EntityNotFound("El estado de la tarea"),
        //             suggestion = "Intente eliminar uno diferente."
        //         });
        //     }

        //     _context.TaskStatuses.Remove(taskStatusModel);
        //     await _context.SaveChangesAsync();
        //     return Ok(new
        //     {
        //         message = MessageConstants.EntityDeleted($"El estado de la tarea '{taskStatusModel.status}'")
        //     });
        // }
        
    }
}