using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BytLabs.MicroserviceTemplate.Api.ConsoleApp
{
    public static class ConsoleEndpoints
    {
        public record LoginRequest(string Username, string Password);

        public static WebApplication MapConsoleEndpoints(this WebApplication app)
        {
            var cfg = app.Configuration.GetSection(ConsoleAuthConfiguration.SectionName).Get<ConsoleAuthConfiguration>() ?? new();

            // Runtime config the static SPA reads on boot (so config changes need no rebuild).
            app.MapGet("/console/config", () => Results.Json(new
            {
                authMode = cfg.Mode,
                oidc = new { authority = cfg.Oidc.Authority, clientId = cfg.Oidc.ClientId },
                graphqlEndpoint = "/graphql/"
            }));

            // Basic-mode login: validate credentials, issue a self-signed JWT the API's JwtBearer accepts.
            app.MapPost("/auth/login", (LoginRequest req) =>
            {
                if (!string.Equals(cfg.Mode, "Basic", StringComparison.OrdinalIgnoreCase))
                    return Results.BadRequest(new { message = "Basic auth is not enabled." });

                if (req.Username != cfg.Basic.Username || req.Password != cfg.Basic.Password)
                    return Results.Unauthorized();

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg.Basic.JwtSigningKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: cfg.Basic.Issuer,
                    claims: new[] { new Claim(ClaimTypes.Name, req.Username), new Claim("role", "console-admin") },
                    expires: DateTime.UtcNow.AddMinutes(cfg.Basic.ExpiryMinutes),
                    signingCredentials: creds);
                return Results.Json(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            });

            return app;
        }
    }
}
