using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using api.Data;
using api.Models;
using api.Dtos.Login;
using api.Dtos.User;
using api.Custome;
using api.Constants;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly Utils _utils;

        public AuthController(ApplicationDBContext dbContext, Utils utils)
        {
            _dbContext = dbContext;
            _utils = utils;
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register(UserDto userDto)
        {
            // Validate required fields
            if (string.IsNullOrEmpty(userDto.userName) || string.IsNullOrEmpty(userDto.password))
            {
                return BadRequest(new { 
                    isSuccess = false, 
                    message = MessageConstants.Generic.RequiredFields 
                });
            }

            // Check if username already exists
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.userName == userDto.userName);
                
            if (existingUser != null)
            {
                return Conflict(new {
                    isSuccess = false,
                    message = UserConstants.UsernameExists
                });
            }

            // Create new user
            var user = new User
            {
                userName = userDto.userName,
                email = userDto.email,
                password = _utils.EncryptSHA256(userDto.password),
                role = userDto.role,
                birthday = userDto.birthday,
                registrationDate = DateTime.Now
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return user.id != 0 
                ? Ok(new { 
                    isSuccess = true, 
                    message = MessageConstants.EntityCreated("Usuario") 
                })
                : StatusCode(StatusCodes.Status500InternalServerError, new { 
                    isSuccess = false, 
                    message = MessageConstants.Generic.ServerError 
                });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            // Validate required fields
            if (string.IsNullOrEmpty(loginDto.userName) || string.IsNullOrEmpty(loginDto.password))
            {
                return BadRequest(new { 
                    isSuccess = false, 
                    message = MessageConstants.Generic.RequiredFields 
                });
            }

            var user = await _dbContext.Users
                .Where(u => u.userName == loginDto.userName &&
                            u.password == _utils.EncryptSHA256(loginDto.password))
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return Unauthorized(new { 
                    isSuccess = false, 
                    message =  AuthConstants.InvalidCredentials 
                });
            }

            return Ok(new { 
                isSuccess = true, 
                token = _utils.GenerateJWT(user),
                message = AuthConstants.LoginSuccess
            });
        }
    }
}