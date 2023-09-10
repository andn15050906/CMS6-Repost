using CMSnet6.Services.Options;
using CMSnet6.Helpers.CookieConfig;

namespace CMSnet6.Helpers
{
    public class Configurer
    {
        private static readonly Dictionary<string, string> configuration = new();

        public static void GetConfiguration()
        {
            string[] config = File.ReadAllLines(".env");
            int assignIndex;

            foreach (string s in config)
                if (!string.IsNullOrEmpty(s))
                {
                    assignIndex = s.IndexOf("=");
                    configuration.Add(s[..assignIndex], s[(assignIndex + 1)..s.Length]);
                }
        }

        public static string GetConnectionString()
        {
            return configuration[configuration["TARGETCONNECTIONSTRING"]];
        }

        public static JwtOptions GetJwtOptions()
        {
            return new JwtOptions
            {
                Audience = configuration["JWT_AUDIENCE"],
                Issuer = configuration["JWT_ISSUERS"],
                Secret = configuration["JWT_SECRET"],
                Lifetime = int.Parse(configuration["JWT_LIFETIME"])
            };
        }

        public static CookieConfigOptions GetCookieConfigOptions()
        {
            return new CookieConfigOptions
            {
                Secure = configuration["COOKIE_SECURE"] == "true",
                Expires = int.Parse(configuration["COOKIE_EXPIRES"])
            };
        }

        public static string[] GetCORS()
        {
            return configuration["CORS"].Split(";");
        }
    }
}
