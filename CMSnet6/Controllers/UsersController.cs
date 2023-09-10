using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using CMSnet6.Controllers.DTOs.User;
using CMSnet6.Models;
using CMSnet6.Models.EntityModels.UserModels;
using CMSnet6.Services.Providers;
using CMSnet6.Helpers;
using CMSnet6.Helpers.CookieConfig;

namespace CMSnet6.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UnitOfWork _uow;

        public UsersController(UnitOfWork uow)
        {
            _uow = uow;
        }






        [HttpPost]
        public IActionResult Register(RegisterDTO dto)
        {
            StatusMessage result = _uow.UserRepo.Create(dto);

            if (result == StatusMessage.Created)
            {
                _uow.Save();
                return Ok();
            }
            return Conflict();
        }

        [HttpPost("/api/[controller]/Login")]
        public IActionResult LogIn([FromBody] LoginDTO dto, [FromServices] JwtProvider jwtProvider)
        {
            (User?, StatusMessage) loginStatus = _uow.UserRepo.CheckLogin(dto);

            if (loginStatus.Item2 == StatusMessage.Unauthorized)
                return Unauthorized();
            if (loginStatus.Item2 == StatusMessage.Forbidden)
                return Forbid();

            UpdateToken(jwtProvider, loginStatus.Item1!);
            _uow.Save();
            return Ok();
        }

        [HttpPost("/api/[controller]/Logout")]
        public IActionResult LogOut()
        {
            CookieOptions options = CookieConfigurer.GetExpiredOptions();
            HttpContext.Response.Cookies.Append("Bearer", "", options);
            HttpContext.Response.Cookies.Append("Refresh", "", options);
            return Ok();
        }

        [HttpPost("/api/[controller]/Refresh")]
        public IActionResult Refresh([FromServices] JwtProvider jwtProvider)
        {
            string? accessToken = CookieParser.GetAccessToken(HttpContext);
            string? refreshToken = CookieParser.GetRefreshToken(HttpContext);

            if (accessToken == null)
                return Unauthorized();
            if (refreshToken == null)
                return Unauthorized("Invalid refresh token");
            var principle = jwtProvider.GetPrincipleFromExpiredToken(accessToken);
            if (principle == null)
                return Unauthorized("Invalid access token");
            User? user = _uow.UserRepo.FindByClaims(principle);
            if (user == null)
                return Unauthorized();
            //always-on refresh token
            if (!_uow.UserRepo.CheckRefreshToken(user, refreshToken))
                return Unauthorized("Invalid refresh token");

            UpdateToken(jwtProvider, user);
            _uow.Save();
            return Ok();
        }

        [HttpPost("/api/[controller]/ChangePassword")]
        public IActionResult ChangePassword(ChangePasswordDTO dto)
        {
            StatusMessage result = _uow.UserRepo.ChangePassword(dto, CookieParser.GetEmail(HttpContext));

            switch (result) {
                case StatusMessage.Unauthorized:
                    return Unauthorized();
                case StatusMessage.Ok:
                    _uow.Save();
                    return Ok();
                default:
                    return BadRequest();
            }
        }

        //Forgot Password?

        //Reset Password?






        [HttpGet]
        [Authorize]
        public IActionResult Get() => Ok(_uow.UserRepo.All().ToList());






        private void UpdateToken(JwtProvider jwtProvider, User user)
        {
            string authToken = jwtProvider.GenerateAccessToken(user.Email!, user.Role.ToString());
            string refreshToken = jwtProvider.GenerateRefreshToken();

            _uow.UserRepo.UpdateRefreshToken(user, refreshToken);

            CookieOptions options = CookieConfigurer.GetOptions();
            HttpContext.Response.Cookies.Append("Bearer", authToken, options);
            HttpContext.Response.Cookies.Append("Refresh", refreshToken, options);
        }
    }
}
