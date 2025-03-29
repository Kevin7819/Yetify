using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Activity;
using api.Dtos.Course;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace api.Controllers
{
    [Authorize]
    [Route("api/Activity")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public ActivityController(ApplicationDBContext context){
            _context = context;
        }
        /**
        *   Get All activities
        **/

        [HttpGet]
        public async Task<IActionResult> GetAllActivities(){
            var activities = await _context.Activities.ToListAsync();
            if (!activities.Any()){
                return NotFound(new {message = "No hay actividades registradas"});
            }
            var activitiesDto = activities.Select(actv => actv.toDto());
            return Ok(activitiesDto);
        }
        /**
        *   Get activity by id
        **/
        [HttpGet("{id}")]
        public async Task<IActionResult> GetActivityById([FromRoute] int id){
            var activity = await _context.Activities.FirstOrDefaultAsync(actv => actv.id == id);
            if (activity == null)
                return NotFound(new {error = "No se encontro las actividades", suggestion = "Intentelo de nuevo"});
            return Ok(activity.toDto());
        }
        /**
        *   Create activity
        **/
        [HttpPost]
        public async Task<IActionResult> CreateActivity([FromBody] CreateActivityRequestDto activityRequestDto){

            if(activityRequestDto == null)
                return BadRequest(new {message = "Se requiere llenar todos los datos necesarios para la actividad"});

            if(string.IsNullOrWhiteSpace(activityRequestDto.activityName))
                return BadRequest(new {message = "Se requiere de un nombre para la actividad"});

            if(string.IsNullOrWhiteSpace(activityRequestDto.activityType))
                return BadRequest(new {message = "Se requiere indicar el tipo de actividad"});

            if(activityRequestDto.identifyActivity == 0)
                return BadRequest(new {message = "Se requiere de un identificador para la actividad."});

            if(activityRequestDto.creationDate <= DateTime.Now)
                return BadRequest(new {message="La fecha debe ser actual o antes de la actual."});
            
            if(string.IsNullOrWhiteSpace(activityRequestDto.activityStatus))
                return BadRequest(new {message="Se requiere de indicar el estado de la actividad"});
            
            if(string.IsNullOrWhiteSpace(activityRequestDto.apiSource))
                return BadRequest(new {message = "Se requiere de indicar el recurso para la actividad"});

            
            var activity = activityRequestDto.ToActivityFromCreateDto();
            await _context.Activities.AddAsync(activity);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetActivityById),new {id = activity.id}, activity.toDto());
        }
        /**
        *   Update activity by id
        **/
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateActivity([FromRoute] int id, [FromBody] UpdateActivityRequestDto updateActivityRequest){
            if(updateActivityRequest == null)
                return BadRequest(new {message = "Se requiere de los datos necesarios para la actividad"});

            if(string.IsNullOrWhiteSpace(updateActivityRequest.activityStatus))
                return BadRequest(new {message="Se requiere de indicar el estado de la actividad."});
            
            var activity = await _context.Activities.FirstOrDefaultAsync(act => act.id == id);
            if (activity == null) { return NotFound(new {error = "No se encontro la actividad", suggestion = "Intentelo de nuevo."}); }
            activity.ToActivityFromUpdateDto(updateActivityRequest);
            await _context.SaveChangesAsync();
            return Ok(activity.toDto());
        }
        /**
        *   Delete activity by id
        **/
        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteActivity([FromRoute]int id){
            var activity = await _context.Activities.FirstOrDefaultAsync(act => act.id == id);
            if (activity == null) return NotFound(new {error = "No se encontro la actividad", suggestion = "Intentelo de nuevo"});
            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();
            return Ok(new {message = "Se elimino la actividad correctamente"});
        }
    }
}