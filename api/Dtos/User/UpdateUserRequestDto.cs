namespace api.Dtos.User 
{
    // DTO for updating existing user data
    public class UpdateUserRequestDto
    {
        // Updated username (must remain unique)
        public string userName { get; set; }

        // New password (should be hashed before storage)
        public string password { get; set; }

        // Updated email (must remain unique)
        public string email { get; set; }

        // Updated birth date
        public DateTime birthday { get; set; }
    }
}