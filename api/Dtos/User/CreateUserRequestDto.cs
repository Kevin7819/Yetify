namespace api.Dtos.User
{
    public class CreateUserRequestDto
    {
        public string userName { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        public string email { get; set; }
        public DateTime birthday { get; set; }
        public DateTime registrationDate { get; set; }
    }
}
