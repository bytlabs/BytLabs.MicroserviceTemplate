using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BytLabs.MicroserviceTemplate.Api.Extensions
{
    // RECIPE: API configuration extension for JWT bearer authentication.
    // Configure via appsettings:
    //   "Authentication": { "Authority": "https://your-idp/", "Audience": "your-api" }
    // When no Authority is configured the scheme is still registered but validation is skipped,
    // so the template runs out-of-the-box; protected ([Authorize]) fields require a token.
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var authority = configuration["Authentication:Authority"];
            var audience = configuration["Authentication:Audience"];

            var authBuilder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
            authBuilder.AddJwtBearer(options =>
            {
                if (!string.IsNullOrWhiteSpace(authority))
                {
                    options.Authority = authority;
                    options.Audience = audience;
                }
                options.TokenValidationParameters.ValidateAudience = !string.IsNullOrWhiteSpace(audience);
            });

            services.AddAuthorization();
            return services;
        }
    }
}
