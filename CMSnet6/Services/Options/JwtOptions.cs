namespace CMSnet6.Services.Options
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = "";
        public string Audience { get; set; } = "";
        public string Secret { get; set; } = "";
        public int Lifetime { get; set; }
    }
}
