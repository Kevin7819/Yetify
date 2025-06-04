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
                    // Por seguridad, no revelar si el email existe
                    _logger.LogInformation($"Solicitud de recuperación para email no registrado: {request.Email}");
                    return Ok(new
                    {
                        isSuccess = true,
                        message = "Si el email existe, se ha enviado un código de recuperación"
                    });
                }

                // Generar token de recuperación
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                // Crear enlace de recuperación
                var resetLink = $"{Request.Scheme}://{Request.Host}/reset-password?token={encodedToken}&email={WebUtility.UrlEncode(user.Email)}";

                // Plantilla de email profesional
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
                    // Por seguridad, no revelar si el email existe
                    return Ok(new
                    {
                        isSuccess = true,
                        message = "Contraseña restablecida exitosamente"
                    });
                }

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
