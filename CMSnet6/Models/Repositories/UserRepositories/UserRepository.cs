using System.Text;
using System.Security.Cryptography;

using CMSnet6.Controllers.DTOs.User;
using CMSnet6.Models.EntityModels;
using CMSnet6.Models.EntityModels.UserModels;
using System.Security.Claims;

namespace CMSnet6.Models.Repositories.UserRepositories
{
    public class UserRepository : BaseRepository<User>
    {
        private readonly int MAX_ACCESS_FAILED_COUNT = 10;

        public UserRepository(Context context) : base(context) { }






        public StatusMessage Create(RegisterDTO dto)
        {
            if (_context.Users.Any(u => u.Email == dto.Email))
                return StatusMessage.Conflict;

            User user = new()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = dto.UserName,
                Password = HashPassword(dto.Password),
                Email = dto.Email,
                JoinDate = DateTime.Now,
                Role = Role.User,
                UpdatedAt = DateTime.Now
            };
            _context.Users.Add(user);
            return StatusMessage.Created;
        }






        public (User?, StatusMessage) CheckLogin(LoginDTO dto)
        {
            User? user = _context.Users.FirstOrDefault(ele => ele.UserName == dto.UserNameOrEmail);

            if (user == null) {
                user = FindByEmail(dto.UserNameOrEmail);
                if (user == null)
                    return (null, StatusMessage.Unauthorized);
            }

            if (user.AccessFailedCount >= MAX_ACCESS_FAILED_COUNT)
                return (null, StatusMessage.Forbidden);

            if (user.Password != HashPassword(dto.Password))
            {
                if (user.AccessFailedCount <= MAX_ACCESS_FAILED_COUNT)
                {
                    user.AccessFailedCount++;
                }
                return (null, StatusMessage.Unauthorized);
            }

            if (user.AccessFailedCount > 0)
            {
                user.AccessFailedCount = 0;
            }
            return (user, StatusMessage.Ok);
        }

        public void UpdateRefreshToken(User user, string token)
        {
            user.Refresh = token;
            user.UpdatedAt = DateTime.Now;
        }

        public bool CheckRefreshToken(User user, string token) => user.Refresh == token;

        public StatusMessage ChangePassword(ChangePasswordDTO dto, string? mail)
        {
            if (mail == null)
                return StatusMessage.Unauthorized;
            if (String.IsNullOrEmpty(dto.CurrentPassword) || String.IsNullOrEmpty(dto.NewPassword))
                return StatusMessage.BadRequest;
            User? user = FindByEmail(mail);
            if (user == null || HashPassword(dto.CurrentPassword) != user.Password)
                return StatusMessage.Unauthorized;
            user.Password = HashPassword(dto.NewPassword);
            return StatusMessage.Ok;
        }






        // ~ CookieParser
        public User? FindByClaims(ClaimsPrincipal userClaim)
        {
            //NameIdentifier == Email
            foreach (Claim claim in userClaim.Claims)
                if (claim.Type == ClaimTypes.NameIdentifier)
                    return _context.Users.FirstOrDefault(u => u.Email == claim.Value);
            return null;
        }



        private static string HashPassword(string password)
        {
            using SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new();
            for (int i = 0; i < bytes.Length; i++)
                builder.Append(bytes[i].ToString("x2"));
            return builder.ToString();
        }

        private User? FindByEmail(string mail)
        {
            return _context.Users.FirstOrDefault(u => u.Email == mail);
        }
    }
}
