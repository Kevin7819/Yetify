using api.Data;
using api.Dtos.User;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using api.Constants;

namespace api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public UserController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users.ToListAsync();
            
            if (!users.Any())
            {
                return NotFound(new { message = MessageConstants.EntityNotFound("usuarios") });
            }

            var usersDto = users.Select(user => user.ToDto());
            return Ok(usersDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest(new 
                {
                    error = MessageConstants.FieldRequired("ID de usuario válido"),
                    suggestion = "El ID debe ser un número positivo."
                });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            
            if (user == null) 
            {
                return NotFound(new
                {
                    error = MessageConstants.EntityNotFound("Usuario"),
                    suggestion = MessageConstants.Generic.TryAgain
                });
            }
            
            return Ok(user.ToDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequestDto userDto)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(userDto.userName))
            {
                return BadRequest(new
                {
                    error = MessageConstants.FieldRequired("Nombre de usuario"),
                    suggestion = "Por favor, ingrese un nombre de usuario válido."
                });
            }

            if (string.IsNullOrWhiteSpace(userDto.password))
            {
                return BadRequest(new
                {
                    error = MessageConstants.FieldRequired("Contraseña"),
                    suggestion = "Por favor, ingrese una contraseña válida."
                });
            }

            if (string.IsNullOrWhiteSpace(userDto.email))
            {
                return BadRequest(new
                {
                    error = MessageConstants.FieldRequired("Email"),
                    suggestion = "Por favor, ingrese un email válido."
                });
            }

            // Check username uniqueness
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName.ToLower() == userDto.userName.ToLower());
            
            if (existingUser != null)
            {
                return Conflict(new
                {
                    error = $"El nombre de usuario '{userDto.userName}' ya está en uso.",
                    suggestion = "Intente con un nombre de usuario diferente."
                });
            }

            // Check email uniqueness
            var existingEmail = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == userDto.email.ToLower());
            
            if (existingEmail != null)
            {
                return Conflict(new
                {
                    error = $"El email '{userDto.email}' ya está registrado.",
                    suggestion = "Intente con un email diferente o recupere su contraseña si ya tiene una cuenta."
                });
            }

            // Create and save new user
            var userModel = userDto.ToUserFromCreateDto();
            await _context.Users.AddAsync(userModel);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetById), new { id = userModel.Id }, 
                new { 
                    message = MessageConstants.EntityCreated($"El usuario '{userModel.UserName}'"), 
                    user = userModel.ToDto() 
                });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserRequestDto userDto)
        {
            // Validate ID
            if (id <= 0)
            {
                return BadRequest(new 
                {
                    error = MessageConstants.FieldRequired("ID de usuario válido"),
                    suggestion = "El ID debe ser un número positivo."
                });
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(userDto.userName))
            {
                return BadRequest(new
                {
                    error = MessageConstants.FieldRequired("Nombre de usuario"),
                    suggestion = "Por favor, ingrese un nombre de usuario válido."
                });
            }

            if (string.IsNullOrWhiteSpace(userDto.password))
            {
                return BadRequest(new
                {
                    error = MessageConstants.FieldRequired("Contraseña"),
                    suggestion = "Por favor, ingrese una contraseña válida."
                });
            }

            if (string.IsNullOrWhiteSpace(userDto.email))
            {
                return BadRequest(new
                {
                    error = MessageConstants.FieldRequired("Email"),
                    suggestion = "Por favor, ingrese un email válido."
                });
            }

            // Find user to update
            var userModel = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            
            if (userModel == null) 
            {
                return NotFound(new
                {
                    error = MessageConstants.EntityNotFound("Usuario"),
                    suggestion = MessageConstants.Generic.TryAgain
                });
            }

            // Check if new username is unique
            if (userModel.UserName.ToLower() != userDto.userName.ToLower())
            {
                var userWithSameName = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName.ToLower() == userDto.userName.ToLower() && u.Id != id);
                
                if (userWithSameName != null)
                {
                    return Conflict(new
                    {
                        error = $"El nombre de usuario '{userDto.userName}' ya está en uso.",
                        suggestion = "Intente con un nombre de usuario diferente."
                    });
                }
            }

            // Check if new email is unique
            if (userModel.Email.ToLower() != userDto.email.ToLower())
            {
                var userWithSameEmail = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == userDto.email.ToLower() && u.Id != id);
                
                if (userWithSameEmail != null)
                {
                    return Conflict(new
                    {
                        error = $"El email '{userDto.email}' ya está registrado.",
                        suggestion = "Intente con un email diferente."
                    });
                }
            }

            // Update user properties
            userModel.UserName = userDto.userName;
            userModel.Role = userDto.role;
            userModel.Email = userDto.email;
            userModel.Birthday = userDto.birthday;

            await _context.SaveChangesAsync();
            
            return Ok(new { 
                message = MessageConstants.EntityUpdated($"El usuario '{userModel.UserName}'"), 
                user = userModel.ToDto() 
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            // Validate ID
            if (id <= 0)
            {
                return BadRequest(new 
                {
                    error = MessageConstants.FieldRequired("ID de usuario válido"),
                    suggestion = "El ID debe ser un número positivo."
                });
            }

            // Find user to delete
            var userModel = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            
            if (userModel == null) 
            {
                return NotFound(new
                {
                    error = MessageConstants.EntityNotFound("Usuario"),
                    suggestion = MessageConstants.Generic.TryAgain
                });
            }

            // Remove user
            _context.Users.Remove(userModel);
            await _context.SaveChangesAsync();
            
            return Ok(new { 
                message = MessageConstants.EntityDeleted($"El usuario '{userModel.UserName}'") 
            });
        }
    }
}