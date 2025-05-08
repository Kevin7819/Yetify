namespace api.Dtos.Login
{
    public class LoginResponseDto
    {
        public int id { get; set; }
        public string userName { get; set; }
        public string email { get; set; }
        public string role { get; set; }
        public string token { get; set; }
    }
}
