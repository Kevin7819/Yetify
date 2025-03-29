using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Config;

namespace api.Services
{
    // Service responsible for JWT authentication and token generation
    public class AuthService
    {
        private readonly JwtSettings _jwtSettings;

        // Constructor receives JWT settings from configuration
        public AuthService(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        // Generates a JWT token for authenticated users
        public string GenerateJwtToken(string userId, string email)
        {
            // Create a security key using the secret key
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Define token claims (user identity details)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId), // Unique user ID
                new Claim(ClaimTypes.Email, email)            // User email
            };

            // Build the token with the specified settings
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(60), // Token expiration time
                signingCredentials: credentials
            );

            // Return the generated JWT token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
