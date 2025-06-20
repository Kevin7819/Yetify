using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;
using api.Data;
using api.Models;
using api.Dtos.Login;
using api.Dtos.User;
using api.Custome;
using api.Constants;
using api.Services;
using System.Net;

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
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            ApplicationDBContext dbContext,
            Utils utils,
            UserManager<User> userManager,
            IEmailSender emailSender,
            ILogger<AuthController> logger)
        {
            _dbContext = dbContext;
            _utils = utils;
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint for user registration
        /// </summary>
        /// <param name="userDto">User data including username, email, password and birthday</param>
        /// <returns>Success response with user ID or error messages</returns>
        [HttpPost("Register")]
        public async Task<ActionResult> Register(UserDto userDto)
        {
            // Validate model state
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
                // Check if username already exists
                var existingUserByName = await _userManager.FindByNameAsync(userDto.userName);
                if (existingUserByName != null)
                {
                    return Conflict(new
                    {
                        isSuccess = false,
                        message = UserConstants.UsernameExists
                    });
                }

                // Check if email already exists
                var existingUserByEmail = await _userManager.FindByEmailAsync(userDto.email);
                if (existingUserByEmail != null)
                {
                    return Conflict(new
                    {
                        isSuccess = false,
                        message = "El email ya está registrado"
                    });
                }

                // Create new user object
                var user = new User
                {
                    UserName = userDto.userName,
                    Email = userDto.email,
                    Birthday = userDto.birthday,
                    RegistrationDate = DateTime.Now
                };

                // Attempt to create user with password
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

        /// <summary>
        /// Endpoint for user authentication
        /// </summary>
        /// <param name="loginDto">Credentials (username and password)</param>
        /// <returns>JWT token and user data if successful, error otherwise</returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            // Validate required fields
            if (string.IsNullOrEmpty(loginDto.userName) || string.IsNullOrEmpty(loginDto.password))
            {
                return BadRequest(new
                {
                    isSuccess = false,
                    message = MessageConstants.Generic.RequiredFields
                });
            }

            // Find user and validate password
            var user = await _userManager.FindByNameAsync(loginDto.userName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.password))
            {
                return Unauthorized(new
                {
                    isSuccess = false,
                    message = AuthConstants.InvalidCredentials
                });
            }

            // Create login response with JWT token
            var loginResponse = new LoginResponseDto
            {
                id = user.Id,
                userName = user.UserName!,
                email = user.Email!,
                token = _utils.GenerateJWT(user)
            };

            return Ok(new
            {
                isSuccess = true,
                message = AuthConstants.LoginSuccess,
                user = loginResponse
            });
        }

        
        /// <summary>
        /// Endpoint to send 6-digit password reset code via email
        /// </summary>
        /// <param name="request">User's email address</param>
        /// <returns>Success message (always returns success for security)</returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> SendResetCode([FromBody] ForgotPasswordDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Security: Always return success to prevent email enumeration
                return Ok(new { isSuccess = true, message = "Si el email existe, se ha enviado un código" });
            }

            // Generate random 6-digit code
            var code = new Random().Next(100000, 999999).ToString();

            // Create reset code entity
            var resetCode = new PasswordResetCode
            {
                UserId = user.Id,
                Code = code,
                Expiration = DateTime.UtcNow.AddMinutes(10) // Code valid for 10 minutes
            };

            // Remove any existing codes for this user
            var oldCodes = _dbContext.PasswordResetCodes.Where(c => c.UserId == user.Id);
            _dbContext.PasswordResetCodes.RemoveRange(oldCodes);

            // Add new code and save
            _dbContext.PasswordResetCodes.Add(resetCode);
            await _dbContext.SaveChangesAsync();

            // URL base for images
            string baseUrl = "https://res.cloudinary.com/dyes5adqo/image/upload/v1749961816";

            // Create email with design for Yetify
            string emailBody = $@"
            <div style='font-family: Comic Sans MS, Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 3px dashed #aee1f9; border-radius: 15px; overflow: hidden; background-color: #f0faff;'>
                <div style='background-color: #d0f0ff; padding: 20px; text-align: center;'>
                    <img src='{baseUrl}/imagenOne-Photoroom_bxq5yj.png' alt='Celular con candado' style='max-width: 120px; border-radius: 10px; margin-bottom: 10px;'>
                    <h2 style='color: #007bff; margin-top: 0;'>¡Hola {user.UserName}!</h2>
                </div>
                <div style='padding: 25px; text-align: center;'>
                    <p style='font-size: 18px;'>Tu amiguito Yetify te ayuda a recuperar tu contraseña 🐾</p>
                    <p style='font-size: 16px;'>Aquí tienes tu código mágico de recuperación:</p>
                    <div style='display: inline-block; background-color: #ffb347; color: #ffffff; font-size: 36px; padding: 20px 50px; border-radius: 12px; letter-spacing: 5px; font-weight: bold; margin: 20px 0; box-shadow: 2px 2px 10px rgba(0,0,0,0.2);'>
                        {code}
                    </div>
                    <p style='font-size: 16px;'>Este código es válido por <b>10 minutos</b>. ¡No tardes en usarlo!</p>
                    <p style='font-size: 14px; color: #555;'>Si no solicitaste este código, puedes ignorar este mensaje.</p>
                </div>
                <div style='background-color: #d0f0ff; padding: 15px; text-align: center;'>
                    <img src='{baseUrl}/yetifylogo_qqasnx.png' alt='Yetify Logo' style='max-width: 100px; opacity: 0.85; margin-bottom: 8px;'>
                    <div style='font-size: 12px; color: #6c757d;'>
                        © {DateTime.Now.Year} Yetify. Todos los derechos reservados.
                    </div>
                </div>
            </div>";

            await _emailSender.SendEmailAsync(user.Email, "Tu Código de Recuperación - Yetify", emailBody);

            return Ok(new { isSuccess = true, message = "Código de recuperación enviado" });
        }


        /// <summary>
        /// Endpoint to reset password using 6-digit verification code
        /// </summary>
        /// <param name="request">Email, verification code and new password</param>
        /// <returns>Success or error message</returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordWithCode([FromBody] ResetPasswordWithCodeDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new { isSuccess = false, message = "Código inválido o expirado" });
            }

            // Validate code exists and hasn't expired
            var codeEntry = await _dbContext.PasswordResetCodes
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.Code == request.Code);

            if (codeEntry == null || codeEntry.Expiration < DateTime.UtcNow)
            {
                return BadRequest(new { isSuccess = false, message = "Código inválido o expirado" });
            }

            // Remove old password and set new one
            var removePassword = await _userManager.RemovePasswordAsync(user);
            var result = await _userManager.AddPasswordAsync(user, request.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    isSuccess = false,
                    message = "Error al restablecer la contraseña",
                    errors = result.Errors.Select(e => e.Description)
                });
            }

            // Remove used code
            _dbContext.PasswordResetCodes.Remove(codeEntry);
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                isSuccess = true,
                message = "Contraseña restablecida correctamente"
            });
        }

        /// <summary>
        /// Test endpoint for email sending functionality
        /// </summary>
        /// <returns>Success message</returns>
        [HttpGet("test-email")]
        public async Task<IActionResult> TestEmail()
        {
            string toEmail = "kvenegasbermudez@gmail.com";
            string subject = "Prueba de correo";
            string htmlMessage = "<h1>Correo de prueba desde Yetify</h1><p>Este es un mensaje de prueba.</p>";

            await _emailSender.SendEmailAsync(toEmail, subject, htmlMessage);
            return Ok("Correo de prueba enviado");
        }
    }
}