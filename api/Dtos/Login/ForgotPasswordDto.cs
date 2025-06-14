using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}