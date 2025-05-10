using System.ComponentModel.DataAnnotations;

namespace api.Dtos.User
{
    // User Data Transfer Object (DTO) for API responses
    // Contains user details without sensitive data
    public class UserDto
    {
        public int id { get; set; }
        
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres")]
        public string userName { get; set; }
        
        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string password { get; set; }
        
        [Required(ErrorMessage = "El rol es requerido")]
        public string role { get; set; }
        
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string email { get; set; }
        
        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        public DateTime birthday { get; set; }
        
        public DateTime registrationDate { get; set; }
    }
}