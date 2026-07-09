namespace BytLabs.MicroserviceTemplate.Api.ConsoleApp
{
    // Configures the bundled admin console's client auth. Bound from the "ConsoleAuth" appsettings
    // section. Mode "None" keeps the template runnable out of the box.
    public class ConsoleAuthConfiguration
    {
        public const string SectionName = "ConsoleAuth";
        public string Mode { get; set; } = "None"; // Basic | Oidc | None
        public BasicAuthOptions Basic { get; set; } = new();
        public OidcOptions Oidc { get; set; } = new();
    }

    public class BasicAuthOptions
    {
        public string Username { get; set; } = "admin";
        public string Password { get; set; } = "";
        public string JwtSigningKey { get; set; } = "";
        public string Issuer { get; set; } = "bytlabs-console";
        public int ExpiryMinutes { get; set; } = 480;
    }

    public class OidcOptions
    {
        public string Authority { get; set; } = "";
        public string ClientId { get; set; } = "";
    }
}
