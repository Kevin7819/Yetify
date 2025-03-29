namespace api.Dtos.User
{
    // DTO for user creation requests
    public class CreateUserRequestDto
    {
        // Required unique username
        public string userName { get; set; }
        
        // Required password (should be hashed before storage)
        public string password { get; set; }
        
        // User role (e.g., "Admin", "User")
        public string role { get; set; }
        
        // Required unique email
        public string email { get; set; }
        
        // User's date of birth
        public DateTime birthday { get; set; }
        
        // Auto-set registration date
        public DateTime registrationDate { get; set; }
    }
}