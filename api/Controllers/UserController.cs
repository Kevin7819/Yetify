using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.User;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace api.Controllers
{
    // Controller requires authentication
    [Authorize]
    // Base route for all endpoints
    [Route("api/user")]
    // API controller with built-in features
    [ApiController]
    public class UserController : ControllerBase
    {
        // Database context instance
        private readonly ApplicationDBContext _context;

        // Constructor with dependency injection
        public UserController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/user - Get all users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users.ToListAsync();
            
            if (!users.Any())
            {
                return NotFound(new { message = "No hay usuarios registrados." });
            }

            var usersDto = users.Select(user => user.ToDto());
            return Ok(usersDto);
        }

        // GET: api/user/{id} - Get user by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest(new 
                {
                    error = "ID de usuario inválido.",
                    sugerencia = "El ID debe ser un número positivo."
                });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.id == id);
            
            if (user == null) 
            {
                return NotFound(new
                {
                    error = "Usuario no encontrado.",
                    sugerencia = "Verifique el ID e intente nuevamente."
                });
            }
            
            return Ok(user.ToDto());
        }

        // POST: api/user - Create new user
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequestDto userDto)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(userDto.userName))
            {
                return BadRequest(new
                {
                    error = "El nombre de usuario no puede estar vacío.",
                    sugerencia = "Por favor, ingrese un nombre de usuario válido."
                });
            }

            if (string.IsNullOrWhiteSpace(userDto.password))
            {
                return BadRequest(new
                {
                    error = "La contraseña no puede estar vacía.",
                    sugerencia = "Por favor, ingrese una contraseña válida."
                });
            }

            if (string.IsNullOrWhiteSpace(userDto.email))
            {
                return BadRequest(new
                {
                    error = "El email no puede estar vacío.",
                    sugerencia = "Por favor, ingrese un email válido."
                });
            }

            // Check username uniqueness
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.userName.ToLower() == userDto.userName.ToLower());
            
            if (existingUser != null)
            {
                return Conflict(new
                {
                    error = $"El nombre de usuario '{userDto.userName}' ya está en uso.",
                    sugerencia = "Intente con un nombre de usuario diferente."
                });
            }

            // Check email uniqueness
            var existingEmail = await _context.Users
                .FirstOrDefaultAsync(u => u.email.ToLower() == userDto.email.ToLower());
            
            if (existingEmail != null)
            {
                return Conflict(new
                {
                    error = $"El email '{userDto.email}' ya está registrado.",
                    sugerencia = "Intente con un email diferente o recupere su contraseña si ya tiene una cuenta."
                });
            }

            // Create and save new user
            var userModel = userDto.ToUserFromCreateDto();
            await _context.Users.AddAsync(userModel);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetById), new { id = userModel.id }, 
                new { 
                    message = $"El usuario '{userModel.userName}' se ha registrado correctamente.", 
                    user = userModel.ToDto() 
                });
        }

        // PUT: api/user/{id} - Update existing user
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserRequestDto userDto)
        {
            // Validate ID
            if (id <= 0)
            {
                return BadRequest(new 
                {
                    error = "ID de usuario inválido.",
                    sugerencia = "El ID debe ser un número positivo."
                });
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(userDto.userName))
            {
                return BadRequest(new
                {
                    error = "El nombre de usuario no puede estar vacío.",
                    sugerencia = "Por favor, ingrese un nombre de usuario válido."
                });
            }

            if (string.IsNullOrWhiteSpace(userDto.password))
            {
                return BadRequest(new
                {
                    error = "La contraseña no puede estar vacía.",
                    sugerencia = "Por favor, ingrese una contraseña válida."
                });
            }

            if (string.IsNullOrWhiteSpace(userDto.email))
            {
                return BadRequest(new
                {
                    error = "El email no puede estar vacío.",
                    sugerencia = "Por favor, ingrese un email válido."
                });
            }

            // Find user to update
            var userModel = await _context.Users.FirstOrDefaultAsync(u => u.id == id);
            
            if (userModel == null) 
            {
                return NotFound(new
                {
                    error = "Usuario no encontrado.",
                    sugerencia = "Verifique el ID e intente nuevamente."
                });
            }

            // Check if new username is unique
            if (userModel.userName.ToLower() != userDto.userName.ToLower())
            {
                var userWithSameName = await _context.Users
                    .FirstOrDefaultAsync(u => u.userName.ToLower() == userDto.userName.ToLower() && u.id != id);
                
                if (userWithSameName != null)
                {
                    return Conflict(new
                    {
                        error = $"El nombre de usuario '{userDto.userName}' ya está en uso.",
                        sugerencia = "Intente con un nombre de usuario diferente."
                    });
                }
            }

            // Check if new email is unique
            if (userModel.email.ToLower() != userDto.email.ToLower())
            {
                var userWithSameEmail = await _context.Users
                    .FirstOrDefaultAsync(u => u.email.ToLower() == userDto.email.ToLower() && u.id != id);
                
                if (userWithSameEmail != null)
                {
                    return Conflict(new
                    {
                        error = $"El email '{userDto.email}' ya está registrado.",
                        sugerencia = "Intente con un email diferente."
                    });
                }
            }

            // Update user properties
            userModel.userName = userDto.userName;
            userModel.password = userDto.password;
            userModel.role = userDto.role;
            userModel.email = userDto.email;
            userModel.birthday = userDto.birthday;

            await _context.SaveChangesAsync();
            
            return Ok(new { 
                message = $"El usuario '{userModel.userName}' se ha actualizado correctamente.", 
                user = userModel.ToDto() 
            });
        }

        // DELETE: api/user/{id} - Delete user
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            // Validate ID
            if (id <= 0)
            {
                return BadRequest(new 
                {
                    error = "ID de usuario inválido.",
                    sugerencia = "El ID debe ser un número positivo."
                });
            }

            // Find user to delete
            var userModel = await _context.Users.FirstOrDefaultAsync(u => u.id == id);
            
            if (userModel == null) 
            {
                return NotFound(new
                {
                    error = "Usuario no encontrado.",
                    sugerencia = "Verifique el ID e intente nuevamente."
                });
            }

            // Remove user
            _context.Users.Remove(userModel);
            await _context.SaveChangesAsync();
            
            return Ok(new { 
                message = $"El usuario '{userModel.userName}' se ha eliminado correctamente." 
            });
        }
    }
}