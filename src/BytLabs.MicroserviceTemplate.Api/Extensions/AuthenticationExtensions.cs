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
            var console = configuration.GetSection(ConsoleApp.ConsoleAuthConfiguration.SectionName)
                .Get<ConsoleApp.ConsoleAuthConfiguration>() ?? new();

            var authBuilder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
            authBuilder.AddJwtBearer(options =>
            {
                if (!string.IsNullOrWhiteSpace(authority))
                {
                    options.Authority = authority;
                    options.Audience = audience;
                }
                options.TokenValidationParameters.ValidateAudience = !string.IsNullOrWhiteSpace(audience);

                // Basic console mode: accept the self-signed HS256 token issued by /auth/login.
                if (string.Equals(console.Mode, "Basic", StringComparison.OrdinalIgnoreCase)
                    && !string.IsNullOrWhiteSpace(console.Basic.JwtSigningKey))
                {
                    var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(console.Basic.JwtSigningKey));
                    options.TokenValidationParameters.ValidateIssuerSigningKey = true;
                    options.TokenValidationParameters.IssuerSigningKey = key;
                    options.TokenValidationParameters.ValidIssuer = console.Basic.Issuer;
                    options.TokenValidationParameters.ValidateIssuer = true;
                    options.TokenValidationParameters.ValidateAudience = false;
                    options.TokenValidationParameters.ValidateLifetime = true;
                }
            });

            services.AddAuthorization();
            return services;
        }
    }
}
