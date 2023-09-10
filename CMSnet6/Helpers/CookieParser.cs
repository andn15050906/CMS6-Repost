using System.Security.Claims;

namespace CMSnet6.Helpers
{
    public class CookieParser
    {
        public static string? GetEmail(HttpContext httpContext)
        {
            foreach (Claim claim in httpContext.User.Claims)
                if (claim.Type == ClaimTypes.NameIdentifier)
                    return claim.Value;
            return null;
        }

        public static string? GetAccessToken(HttpContext httpContext) => httpContext.Request.Cookies["Bearer"];

        public static string? GetRefreshToken(HttpContext httpContext) => httpContext.Request.Cookies["Refresh"];
    }
}
