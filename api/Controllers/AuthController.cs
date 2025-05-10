using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.Text;

using api.Data;
using api.Models;
using api.Dtos.Login;
using api.Dtos.User;
using api.Custome;
using api.Constants;
using api.Services;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly Utils _utils;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;

        public AuthController(
            ApplicationDBContext dbContext,
            Utils utils,
            UserManager<User> userManager,
            IEmailSender emailSender)
        {
            _dbContext = dbContext;
            _utils = utils;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register(UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    isSuccess = false,
                    message = "Error de validación",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            try
            {
                var existingUserByName = await _userManager.FindByNameAsync(userDto.userName);
                if (existingUserByName != null)
                {
                    return Conflict(new
                    {
                        isSuccess = false,
                        message = UserConstants.UsernameExists
                    });
                }

                var existingUserByEmail = await _userManager.FindByEmailAsync(userDto.email);
                if (existingUserByEmail != null)
                {
                    return Conflict(new
                    {
                        isSuccess = false,
                        message = "El email ya está registrado"
                    });
                }

                var user = new User
                {
                    UserName = userDto.userName,
                    Email = userDto.email,
                    Role = userDto.role,
                    Birthday = userDto.birthday,
                    RegistrationDate = DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, userDto.password);

                if (result.Succeeded)
                {
                    return Ok(new
                    {
                        isSuccess = true,
                        message = MessageConstants.EntityCreated("Usuario"),
                        userId = user.Id
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        isSuccess = false,
                        message = "Error al crear el usuario",
                        errors = result.Errors.Select(e => e.Description)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar usuario: {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    isSuccess = false,
                    message = "Error interno del servidor",
                    detail = ex.Message
                });
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.userName) || string.IsNullOrEmpty(loginDto.password))
            {
                return BadRequest(new
                {
                    isSuccess = false,
                    message = MessageConstants.Generic.RequiredFields
                });
            }

            var user = await _userManager.FindByNameAsync(loginDto.userName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.password))
            {
                return Unauthorized(new
                {
                    isSuccess = false,
                    message = AuthConstants.InvalidCredentials
                });
            }

            var loginResponse = new LoginResponseDto
            {
                id = user.Id,
                userName = user.UserName!,
                email = user.Email!,
                role = user.Role,
                token = _utils.GenerateJWT(user)
            };

            return Ok(new
            {
                isSuccess = true,
                message = AuthConstants.LoginSuccess,
                user = loginResponse
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    isSuccess = false,
                    message = "Invalid data",
                    errors = ModelState.Values.SelectMany(v => v.Errors)
                });
            }

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return BadRequest(new
                {
                    isSuccess = false,
                    message = "Email is required"
                });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return Ok(new
                {
                    isSuccess = true,
                    message = "If the email exists, a recovery link has been sent"
                });
            }

            try
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var resetLink = $"http://localhost:5003/reset-password?token={encodedToken}&email={user.Email}";

                // Enviar el correo SIEMPRE
                var message = $"<p>Click <a href='{resetLink}'>here</a> to reset your password.</p>";
                await _emailSender.SendEmailAsync(user.Email, "Reset your password", message);

                // También incluir el token si quieres verlo en dev (opcional)
                return Ok(new
                {
                    isSuccess = true,
                    message = "Recovery email sent",
                    resetLink = resetLink,
                    token = encodedToken
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    isSuccess = false,
                    message = "An error occurred while sending the email",
                    error = ex.Message
                });
            }
        }



        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    isSuccess = false,
                    message = "Datos inválidos",
                    errors = ModelState.Values.SelectMany(v => v.Errors)
                });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Ok(new
                {
                    isSuccess = true,
                    message = "Contraseña restablecida si el email es válido"
                });
            }
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    isSuccess = false,
                    message = "No se pudo restablecer la contraseña",
                    errors = result.Errors.Select(e => e.Description)
                });
            }

            return Ok(new
            {
                isSuccess = true,
                message = "Contraseña restablecida exitosamente"
            });
        }
        [HttpGet("test-email")]
        public async Task<IActionResult> TestEmail()
        {
            await _emailSender.SendEmailAsync("kvenegasbermudez@gmail.com", "Prueba de envío", "<p>Hola, esto es una prueba.</p>");
            return Ok("Correo enviado (si no hay error)");
        }
    }
}
