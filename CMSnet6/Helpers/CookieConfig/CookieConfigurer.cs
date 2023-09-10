namespace CMSnet6.Helpers.CookieConfig
{
    public class CookieConfigurer
    {
        private static CookieConfigOptions _options = null!;

        private CookieConfigurer() { }

        public static void Create(CookieConfigOptions options)
        {
            _options = options;
        }

        public static CookieOptions GetOptions() => new()
        {
            SameSite = SameSiteMode.None,
            Secure = _options.Secure,
            Expires = DateTime.Now.AddDays(_options.Expires)
        };

        public static CookieOptions GetExpiredOptions() => new()
        {
            SameSite = SameSiteMode.None,
            Secure = _options.Secure,
            Expires = DateTime.Now
        };
    }
}
