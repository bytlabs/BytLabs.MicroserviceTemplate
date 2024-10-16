using BytLabs.MicroserviceTemplate.Client;
using BytLabs.MicroserviceTemplate.Tests.Accpetance.Support;
using OpenTelemetry.Instrumentation.AspNetCore;
using System;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;

namespace BytLabs.MicroserviceTemplate.Tests.Accpetance.StepDefinitions
{
    [Binding]
    public class MarkOrderAsShippedStepDefinitions
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly ScenarioContext _context;
        private readonly IMicroserviceTemplateGraphQLClient _graphqlClient;

        public MarkOrderAsShippedStepDefinitions(ScenarioContext context)
        {
            _factory = new CustomWebApplicationFactory<Program>();
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _graphqlClient = _factory.GetGraphQLClient();
        }

        [Given(@"a valid `markOrderAsShipped` input")]
        public void GivenAValidMarkOrderAsShippedInput()
        {
            throw new PendingStepException();
        }

        [When(@"the `markOrderAsShipped` GraphQL mutation is called")]
        public void WhenTheMarkOrderAsShippedGraphQLMutationIsCalled()
        {
            throw new PendingStepException();
        }

        [Then(@"the order status should be updated to ""([^""]*)""")]
        public void ThenTheOrderStatusShouldBeUpdatedTo(string shipped)
        {
            throw new PendingStepException();
        }

        [Given(@"an invalid `markOrderAsShipped` input")]
        public void GivenAnInvalidMarkOrderAsShippedInput()
        {
            throw new PendingStepException();
        }

        [Then(@"the order status should not be updated")]
        public void ThenTheOrderStatusShouldNotBeUpdated()
        {
            throw new PendingStepException();
        }
    }
}
