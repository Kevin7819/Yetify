using api.Data;
using api.Dtos.Activity;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using api.Constants;

namespace api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        
        public ActivityController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActivities()
        {
            var activities = await _context.Activities.ToListAsync();
            if (!activities.Any())
            {
                return NotFound(new { message = ActivityConstants.NoActivitiesRegistered });
            }
            var activitiesDto = activities.Select(actv => actv.toDto());
            return Ok(activitiesDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetActivityById([FromRoute] int id)
        {
            var activity = await _context.Activities.FirstOrDefaultAsync(actv => actv.id == id);
            if (activity == null)
                return NotFound(new {
                    error = MessageConstants.EntityNotFound("La actividad"),
                    suggestion = MessageConstants.Generic.TryAgain
                });
            return Ok(activity.toDto());
        }

        [HttpPost]
        public async Task<IActionResult> CreateActivity([FromBody] CreateActivityRequestDto activityRequestDto)
        {
            if(activityRequestDto == null)
                return BadRequest(new { message = MessageConstants.Generic.RequiredFields });

            if(string.IsNullOrWhiteSpace(activityRequestDto.activityName))
                return BadRequest(new { message = ActivityConstants.NameRequired });

            if(string.IsNullOrWhiteSpace(activityRequestDto.activityType))
                return BadRequest(new { message = ActivityConstants.TypeRequired });

            if(activityRequestDto.identifyActivity == 0)
                return BadRequest(new { message = ActivityConstants.IdentifyRequired });

            if(activityRequestDto.creationDate <= DateTime.Now)
                return BadRequest(new { message = ActivityConstants.InvalidDate });
            
            if(string.IsNullOrWhiteSpace(activityRequestDto.activityStatus))
                return BadRequest(new { message = ActivityConstants.StatusRequired });
            
            if(string.IsNullOrWhiteSpace(activityRequestDto.apiSource))
                return BadRequest(new { message = ActivityConstants.ApiSourceRequired });

            var activity = activityRequestDto.ToActivityFromCreateDto();
            await _context.Activities.AddAsync(activity);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetActivityById), new { id = activity.id }, 
                new {
                    message = MessageConstants.EntityCreated("La actividad"),
                    data = activity.toDto()
                });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateActivity([FromRoute] int id, [FromBody] UpdateActivityRequestDto updateActivityRequest)
        {
            if(updateActivityRequest == null)
                return BadRequest(new { message = MessageConstants.Generic.RequiredFields });

            if(string.IsNullOrWhiteSpace(updateActivityRequest.activityStatus))
                return BadRequest(new { message = ActivityConstants.StatusRequired });
            
            var activity = await _context.Activities.FirstOrDefaultAsync(act => act.id == id);
            if (activity == null)
            { 
                return NotFound(new {
                    error = MessageConstants.EntityNotFound("La actividad"), 
                    suggestion = MessageConstants.Generic.TryAgain
                }); 
            }
            
            activity.ToActivityFromUpdateDto(updateActivityRequest);
            await _context.SaveChangesAsync();
            
            return Ok(new {
                message = MessageConstants.EntityUpdated("La actividad"),
                data = activity.toDto()
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity([FromRoute] int id)
        {
            var activity = await _context.Activities.FirstOrDefaultAsync(act => act.id == id);
            if (activity == null) 
                return NotFound(new {
                    error = MessageConstants.EntityNotFound("La actividad"),
                    suggestion = MessageConstants.Generic.TryAgain
                });
            
            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();
            
            return Ok(new {
                message = MessageConstants.EntityDeleted("La actividad")
            });
        }
    }
}