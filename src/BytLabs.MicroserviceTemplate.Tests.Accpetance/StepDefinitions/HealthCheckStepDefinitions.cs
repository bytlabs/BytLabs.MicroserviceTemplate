using BytLabs.MicroserviceTemplate.Tests.Accpetance.Support;
using TechTalk.SpecFlow.Infrastructure;

namespace BytLabs.MicroserviceTemplate.Tests.Accpetance.StepDefinitions
{
    [Binding]
    public class HealthCheckStepDefinitions
    {
        private readonly HttpClient _client;
        private HttpResponseMessage? _response;
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly ScenarioContext _context;

        public HealthCheckStepDefinitions(ScenarioContext context)
        {
            _factory = new CustomWebApplicationFactory<Program>();
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _client = _factory.CreateClient();
        }

        [Given(@"the application client is initialized")]
        public void GivenTheApplicationClientIsInitialized()
        {
            // Initialization already handled in the constructor
        }

        [When(@"I check the health endpoint")]
        public async Task WhenICheckTheHealthEndpoint()
        {
            _response = await _client.GetAsync("/healthz");
        }

        [Then(@"the response should indicate the application is healthy")]
        public void ThenTheResponseShouldIndicateTheApplicationIsHealthy()
        {
            _response.EnsureSuccessStatusCode();
        }
    }
}
