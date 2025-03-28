using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using api.Models;

namespace api.Custome
{
    public class Utils
    {
        private readonly IConfiguration _configuration;

        public Utils(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Generates SHA256 hash of input string (one-way encryption)
        // Input string to hash
        // 64-character hexadecimal hash string
        public string EncryptSHA256(string text)
        {
            using (SHA256 sha256Hash = SHA256.Create()) 
            {
                // Convert text to byte array and compute hash
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(text));

                // Convert byte array to hexadecimal string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    // "x2" formats each byte as 2-digit hexadecimal
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        // Generates JWT token for user authentication
        // User object containing authentication data
        // Signed JWT token string
        public string GenerateJWT(User model)
        {
            // User claims to include in token
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, model.id.ToString()),  // Unique user ID
                new Claim(ClaimTypes.Name, model.userName)                // Username
            };

            // Get secret key from configuration
            var jwtKey = _configuration["Jwt:key"];
            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("JWT key is not configured.");

            // Create security key and signing credentials
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            
            // Configure JWT token
            var jwtConfig = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],       // Token issuer
                audience: _configuration["Jwt:Audience"],   // Valid audience
                claims: userClaims,                         // User data
                expires: DateTime.UtcNow.AddMinutes(10),    // Expiration (UTC)
                signingCredentials: credentials             // Signing configuration
            );

            // Serialize token to string
            return new JwtSecurityTokenHandler().WriteToken(jwtConfig);
        }
    }
}