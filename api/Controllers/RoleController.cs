using api.Data;
using api.Dtos.Role;
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
     public class RoleController : ControllerBase
     {
    //     private readonly ApplicationDBContext _context;

    //     public RoleController(ApplicationDBContext context)
    //     {
    //         _context = context;
    //     }

    //     [HttpGet]
    //     public async Task<IActionResult> GetAll()
    //     {
    //         var roles = await _context.Roles.ToListAsync();

    //         if (!roles.Any())
    //         {
    //             return NotFound(new { message = MessageConstants.EntityNotFound("roles") });
    //         }

    //         var rolesDto = roles.Select(role => role.ToDto());
    //         return Ok(rolesDto);
    //     }

    //     [HttpGet("{id}")]
    //     public async Task<IActionResult> GetById([FromRoute] int id)
    //     {
    //         var role = await _context.Roles.FirstOrDefaultAsync(r => r.id == id);
    //         if (role == null)
    //         {
    //             return NotFound(new
    //             {
    //                 error = MessageConstants.EntityNotFound("El rol"),
    //                 suggestion = MessageConstants.Generic.TryAgain
    //             });
    //         }
    //         return Ok(role.ToDto());
    //     }

    //     [HttpPost]
    //     public async Task<IActionResult> Create([FromBody] CreateRoleRequestDto roleDto)
    //     {
    //         if (string.IsNullOrWhiteSpace(roleDto.name))
    //         {
    //             return BadRequest(new
    //             {
    //                 error = MessageConstants.FieldRequired("Nombre del rol"),
    //                 suggestion = "Por favor, ingrese un nombre válido para el rol."
    //             });
    //         }

    //         bool exists = await _context.Roles.AnyAsync(r => r.Name.ToLower() == roleDto.name.ToLower());

    //         if (exists)
    //         {
    //             return Conflict(new
    //             {
    //                 error = $"El rol '{roleDto.name}' ya existe.",
    //                 suggestion = "Intente guardar el rol con un nombre diferente."
    //             });
    //         }

    //         var roleModel = roleDto.ToCourseFromCreateDto();
    //         await _context.Roles.AddAsync(roleModel);
    //         await _context.SaveChangesAsync();

    //         return CreatedAtAction(nameof(GetById), new { id = roleModel.id },
    //             new
    //             {
    //                 message = MessageConstants.EntityCreated($"El rol '{roleModel.name}'"),
    //                 role = roleModel.ToDto()
    //             });
    //     }

    //     [HttpPut("{id}")]
    //     public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CreateRoleRequestDto roleDto)
    //     {
    //         var roleModel = await _context.Roles.FirstOrDefaultAsync(r => r.id == id);
    //         if (roleModel == null)
    //         {
    //             return NotFound(new {
    //                 error = MessageConstants.EntityNotFound("El rol"),
    //                 suggestion = MessageConstants.Generic.TryAgain
    //             });
    //         }

    //         if (string.IsNullOrWhiteSpace(roleDto.name))
    //         {
    //             return BadRequest(new {
    //                 error = MessageConstants.FieldRequired("Nombre del rol"),
    //                 suggestion = "Por favor, ingrese un nombre válido para el rol."
    //             });
    //         }

    //         if (roleModel.Name.ToLower() == roleDto.name.ToLower())
    //         {
    //             await _context.SaveChangesAsync();
    //             return Ok(new
    //             {
    //                 message = MessageConstants.EntityUpdated($"El rol '{roleModel.name}'"),
    //             });
    //         }

    //         bool exists = await _context.Roles.AnyAsync(r => r.name.ToLower() == roleDto.name.ToLower() && r.id != id);
    //         if (exists)
    //         {
    //             return Conflict(new {
    //                 error = $"El rol '{roleDto.name}' ya existe.",
    //                 suggestion = "Intente actualizarlo con un nombre diferente."
    //             });
    //         }
    //         roleModel.Name = roleDto.name;
    //         await _context.SaveChangesAsync();
    //         return Ok(new
    //         {
    //             message = MessageConstants.EntityUpdated($"El rol '{roleModel.name}'"),
    //             role = roleModel.ToDto()
    //         });
    //     }

    //     [HttpDelete("{id}")]

    //     public async Task<IActionResult> Delete([FromRoute] int id)
    //     {
    //         var roleModel = await _context.Roles.FirstOrDefaultAsync(r => r.id == id);
    //         if (roleModel == null)
    //         {
    //             return NotFound(new
    //             {
    //                 error = MessageConstants.EntityNotFound("El rol"),
    //                 suggestion = "Intente eliminar uno diferente."
    //             });
    //         }

    //         _context.Roles.Remove(roleModel);
    //         await _context.SaveChangesAsync();
    //         return Ok(new
    //         {
    //             message = MessageConstants.EntityDeleted($"El rol '{roleModel.name}'")
    //         });
    //     }

     }
    
}