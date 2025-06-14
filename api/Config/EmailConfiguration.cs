namespace api.Config
{
    public class EmailConfiguration
    {
        public string From { get; set; }
        public string DisplayName { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; } = true; 
        public int Timeout { get; set; } = 30000; 
        public string AdminEmail { get; set; } 
        public bool EnableLogging { get; set; } = true; 
    }
}