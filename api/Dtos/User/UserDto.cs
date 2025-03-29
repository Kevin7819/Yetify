namespace api.Dtos.User
{
    // User Data Transfer Object (DTO) for API responses
    // Contains user details without sensitive data
    public class UserDto
    {
        // Unique user identifier
        public int id { get; set; }
        
        // User's display name
        public string userName { get; set; }
        
        // Hashed password (should never be exposed in responses)
        public string password { get; set; }
        
        // User role (e.g., "Admin", "User")
        public string role { get; set; }
        
        // User's email address
        public string email { get; set; }
        
        // User's date of birth
        public DateTime birthday { get; set; }
        
        // When the user registered (set automatically)
        public DateTime registrationDate { get; set; }
    }
}