using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.Text;

using Microsoft.AspNetCore.Hosting;
using System.Text;

using api.Data;
using api.Models;
using api.Dtos.Login;
using api.Dtos.User;
using api.Custome;
using api.Constants;
using api.Services;
using Microsoft.AspNetCore.Identity.Data;
using System.Net;
using api.Services;
using Microsoft.AspNetCore.Identity.Data;
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
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            ApplicationDBContext dbContext,
            Utils utils,
            UserManager<User> userManager,
            IEmailSender emailSender,
            ILogger<AuthController> logger)
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
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint for user registration
        /// </summary>
        /// <param name="userDto">User data including username, email, password, role and birthday</param>
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
                    Role = userDto.role,
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
                role = user.Role,
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

        /// <summary>
        /// Endpoint to initiate password reset process
        /// </summary>
        /// <param name="request">User's email address</param>
        /// <returns>Success message (always returns success for security)</returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
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

            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    // Security: Always return success to prevent email enumeration
                    _logger.LogInformation($"Solicitud de recuperación para email no registrado: {request.Email}");
                    return Ok(new
                    {
                        isSuccess = true,
                        message = "Si el email existe, se ha enviado un código de recuperación"
                    });
                }

                // Generate and encode password reset token
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                // Create reset link
                var resetLink = $"{Request.Scheme}://{Request.Host}/reset-password?token={encodedToken}&email={WebUtility.UrlEncode(user.Email)}";

                // Create professional HTML email template
                var emailBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden;'>
                    <div style='background-color: #f8f9fa; padding: 20px; text-align: center;'>
                        <img src='[api\Assets\images\imagenOne.jpg]' alt='Logo' style='max-width: 150px;'>
                        <h2 style='color: #343a40; margin-top: 15px;'>Recuperación de Contraseña</h2>
                    </div>
                    <div style='padding: 25px;'>
                        <p>Hola {user.UserName},</p>
                        <p>Hemos recibido una solicitud para restablecer tu contraseña. Haz clic en el siguiente botón para continuar:</p>
                        <div style='text-align: center; margin: 25px 0;'>
                            <a href='{resetLink}' style='background-color: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; font-weight: bold; display: inline-block;'>
                                Restablecer Contraseña
                            </a>
                        </div>
                        <p>Si no solicitaste este cambio, por favor ignora este mensaje. El enlace expirará en 1 hora.</p>
                        <p style='color: #6c757d; font-size: 12px; margin-top: 30px;'>
                            ¿No funciona el botón? Copia y pega este enlace en tu navegador:<br>
                            <span style='word-break: break-all;'>{resetLink}</span>
                        </p>
                    </div>
                    <div style='background-color: #f8f9fa; padding: 15px; text-align: center; font-size: 12px; color: #6c757d;'>
                        © {DateTime.Now.Year} {Request.Host}. Todos los derechos reservados.
                    </div>
                </div>";

                await _emailSender.SendEmailAsync(user.Email, "Restablecer tu contraseña", emailBody);

                _logger.LogInformation($"Email de recuperación enviado a {user.Email}");

                return Ok(new
                {
                    isSuccess = true,
                    message = "Si el email existe, se ha enviado un código de recuperación"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar solicitud de recuperación de contraseña");
                return StatusCode(500, new
                {
                    isSuccess = false,
                    message = "Error interno al procesar la solicitud"
                });
            }
        }

        /// <summary>
        /// Endpoint to complete password reset process
        /// </summary>
        /// <param name="request">Reset token, email and new password</param>
        /// <returns>Success or error message</returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
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

            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    // Security: Always return success to prevent email enumeration
                    return Ok(new
                    {
                        isSuccess = true,
                        message = "Contraseña restablecida exitosamente"
                    });
                }

                // Decode token and reset password
                var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
                var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);

                if (!result.Succeeded)
                {
                    _logger.LogWarning($"Error al restablecer contraseña para {request.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    return BadRequest(new
                    {
                        isSuccess = false,
                        message = "No se pudo restablecer la contraseña",
                        errors = result.Errors.Select(e => e.Description)
                    });
                }

                _logger.LogInformation($"Contraseña restablecida exitosamente para {request.Email}");
                return Ok(new
                {
                    isSuccess = true,
                    message = "Contraseña restablecida exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restablecer contraseña");
                return StatusCode(500, new
                {
                    isSuccess = false,
                    message = "Error interno al restablecer la contraseña"
                });
            }
        }

        /// <summary>
        /// Endpoint to send 6-digit password reset code via email
        /// </summary>
        /// <param name="request">User's email address</param>
        /// <returns>Success message (always returns success for security)</returns>
        [HttpPost("send-reset-code")]
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

            // Create email with code
            string emailBody = $@"
                <h2>Recuperación de Contraseña</h2>
                <p>Tu código de verificación es:</p>
                <h1 style='color: #007bff'>{code}</h1>
                <p>Este código expira en 10 minutos.</p>
            ";

            await _emailSender.SendEmailAsync(user.Email, "Código de Recuperación", emailBody);

            return Ok(new { isSuccess = true, message = "Código de recuperación enviado" });
        }

        /// <summary>
        /// Endpoint to reset password using 6-digit verification code
        /// </summary>
        /// <param name="request">Email, verification code and new password</param>
        /// <returns>Success or error message</returns>
        [HttpPost("reset-password-with-code")]
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