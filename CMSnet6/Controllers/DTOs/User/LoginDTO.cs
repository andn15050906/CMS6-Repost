namespace CMSnet6.Controllers.DTOs.User
{
    public class LoginDTO
    {
        public string UserNameOrEmail { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
