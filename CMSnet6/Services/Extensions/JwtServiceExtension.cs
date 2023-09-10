using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using CMSnet6.Services.Options;
using CMSnet6.Services.Providers;

namespace CMSnet6.Services.Extensions
{
    public static class JwtServiceExtension
    {
        public static void AddJwt(this IServiceCollection services, JwtOptions config)
        {
            services.Configure<JwtOptions>(options =>
            {
                options.Issuer = config.Issuer;
                options.Audience = config.Audience;
                options.Secret = config.Secret;
                options.Lifetime = config.Lifetime;
            });

            services.AddTransient<JwtProvider>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidAudience = config.Audience,
                        ValidIssuer = config.Issuer,
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Secret))
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Cookies["Bearer"];
                            return Task.CompletedTask;
                        }
                    };
                })
                .AddCookie(options =>
                {
                    //options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Cookie.IsEssential = true;
                });
        }
    }
}
