namespace CMSnet6.Models.DTOs.User
{
    public class UserDTO
    {
        public string? Id { get; set; }

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public DateTime JoinDate { get; set; }

        public string? Avatar { get; set; }
    }
}
