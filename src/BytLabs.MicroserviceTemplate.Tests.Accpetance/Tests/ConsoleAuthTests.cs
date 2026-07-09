using System.Net;
using BytLabs.MicroserviceTemplate.Tests.Accpetance.Support;

namespace BytLabs.MicroserviceTemplate.Tests.Accpetance.Tests;

public class ConsoleAuthTests
{
    [Fact]
    public async Task Config_endpoint_returns_auth_mode()
    {
        var client = new CustomWebApplicationFactory<Program>().CreateClient();

        var res = await client.GetAsync("/console/config");

        res.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await res.Content.ReadAsStringAsync();
        body.Should().Contain("authMode");
        body.Should().Contain("graphqlEndpoint");
    }
}
