using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using api.Data;
using api.Models;
using api.Dtos.Login;
using api.Dtos.User;
using api.Custome;

namespace api.Controllers
{
    // Controller for user authentication
    [Route("api/[controller]")] // Defines the base route for the controller
    [AllowAnonymous] // Allows access without authentication
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

        // Endpoint to register a new user
        [HttpPost("Register")]
        public async Task<ActionResult> Register(UserDto userDto)
        {
            // Validates that the username and password are not empty
            if (string.IsNullOrEmpty(userDto.userName) || string.IsNullOrEmpty(userDto.password))
            {
                return BadRequest(new { isSuccess = false, message = "Username and password are required." });
            }

            // Creates a new user with an encrypted password
            var user = new User
            {
                userName = userDto.userName,
                email = userDto.email,
                password = _utils.EncryptSHA256(userDto.password), // Encrypts the password
                role = userDto.role,
                birthday = userDto.birthday,
                registrationDate = DateTime.Now
            };

            // Saves the user in the database
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Checks if the user was successfully created
            if (user.id != 0)
                return Ok(new { isSuccess = true, message = "User successfully registered." });
            else
                return Ok(new { isSuccess = false, message = "Error registering the user." });
        }

        // Endpoint for user login and JWT token generation
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            // Searches for the user with the provided credentials
            var user = await _dbContext.Users
                .Where(u => u.userName == loginDto.userName &&
                            u.password == _utils.EncryptSHA256(loginDto.password)) // Encrypts the password for comparison
                .FirstOrDefaultAsync();

            // If the user does not exist, return an empty token
            if (user == null)
                return Ok(new { isSuccess = false, token = "" });

            // If credentials are correct, generate and return a JWT token
            return Ok(new { isSuccess = true, token = _utils.GenerateJWT(user) });
        }
    }
}
