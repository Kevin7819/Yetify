namespace api.Config
{
    // Configuration class for JWT authentication settings
    public class JwtSettings
    {
        public string Key { get; set; }       // Secret key used to sign tokens
        public string Issuer { get; set; }    // Token issuer (API)
        public string Audience { get; set; }  // Intended audience (clients)
    }
}
