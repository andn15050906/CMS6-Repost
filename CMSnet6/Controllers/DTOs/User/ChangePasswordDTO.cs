namespace CMSnet6.Controllers.DTOs.User
{
    public class ChangePasswordDTO
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; }
    }
}
