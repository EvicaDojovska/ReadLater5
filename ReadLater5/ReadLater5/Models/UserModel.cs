namespace ReadLater5.Models
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserModel : LoginModel
    {
        public string Email { get; set; }
    }
}