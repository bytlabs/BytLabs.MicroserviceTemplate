using System.Net.Http.Json;
using BytLabs.MicroserviceTemplate.Tests.Accpetance.Support;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BytLabs.MicroserviceTemplate.Tests.Accpetance.Tests;

public class HealthCheckTests
{
    [Fact]
    public async Task Server_responds_to_a_trivial_graphql_query()
    {
        var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new("http://localhost/graphql")
        });
        client.DefaultRequestHeaders.Add("Tenant", "Test");

        var response = await client.PostAsJsonAsync("", new { query = "{ __typename }" });

        response.IsSuccessStatusCode.Should().BeTrue();
    }
}
