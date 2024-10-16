using BytLabs.MicroserviceTemplate.Client;
using BytLabs.MicroserviceTemplate.Tests.Accpetance.Support;
using Microsoft.Extensions.DependencyInjection;
using StrawberryShake;
using TechTalk.SpecFlow.Infrastructure;

namespace BytLabs.MicroserviceTemplate.Tests.Accpetance.StepDefinitions
{
    [Binding]
    public class CreateOrderStepDefinitions
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly ScenarioContext _context;
        private readonly IMicroserviceTemplateGraphQLClient _graphqlClient;

        public CreateOrderStepDefinitions(ScenarioContext context)
        {
            _factory = new CustomWebApplicationFactory<Program>();
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _graphqlClient = _factory.GetGraphQLClient();
        }

        [Given(@"a valid `createOrder` input")]
        public void GivenAValidCreateOrderInput()
        {
            var orderId = Guid.NewGuid();
            _context.Add("orderId", orderId);
            var createOrderInput = new CreateOrderInputBuilder()
                .WithOrderId(orderId.ToString())
                .WithItems(new List<OrderItemInput>
                {
                    new OrderItemInputBuilder()
                        .WithProductId(GuidExtensions.GUID_0002)
                        .WithQuantity(1)
                        .WithPrice(100)
                        .WithId(GuidExtensions.GUID_0003)
                        .Build(),

                    new OrderItemInputBuilder()
                        .WithProductId(GuidExtensions.GUID_0004)
                        .WithQuantity(1)
                        .WithPrice(100)
                        .WithId(GuidExtensions.GUID_0005)
                        .Build()
                })
                .WithOrderDate(DateTimeExtensions.DDMMYYYY_01_01_2000)
                .Build();

            _context.Add("createOrderInput", createOrderInput);
        }


        [When(@"the `createOrder` GraphQL mutation is called")]
        public async Task WhenTheCreateOrderGraphQLMutationIsCalled()
        {
            var createOrderInput = _context.Get<CreateOrderInput>("createOrderInput");
            var createOrderResult = await _graphqlClient.CreateOrder
                                            .ExecuteAsync(createOrderInput, CancellationToken.None);
            _context.Add("createOrderResult", createOrderResult);
            
        }

        [Then(@"the order should be successfully created")]
        public void ThenTheOrderShouldBeSuccessfullyCreated()
        {
            var createOrderResult = _context.Get<IOperationResult<ICreateOrderResult>>("createOrderResult");
            var orderId = _context.Get<Guid>("orderId");
            createOrderResult.Data.Should().NotBeNull();
            createOrderResult.Data!.CreateOrder.Should().NotBeNull();
            createOrderResult.Data.CreateOrder.CreateOrderResult!.OrderId.Should().Be(orderId.ToString("N"));
            createOrderResult.Data.CreateOrder.Errors.Should().BeNull();
        }

        [Given(@"an invalid `createOrder` input")]
        public void GivenAnInvalidCreateOrderInput()
        {
            var createOrderInput = new CreateOrderInputBuilder()
                .WithOrderId(GuidExtensions.GUID_0002)
                .WithItems(new List<OrderItemInput>())
                .WithOrderDate(DateTimeExtensions.DDMMYYYY_01_01_2000)
                .Build();

            _context.Add("createOrderInput", createOrderInput);
        }

        [Then(@"the order should not be created")]
        public void ThenTheOrderShouldNotBeCreated()
        {
            var createOrderResult = _context.Get<IOperationResult<ICreateOrderResult>>("createOrderResult");
            createOrderResult.Data.Should().NotBeNull();
            createOrderResult.Data!.CreateOrder.CreateOrderResult.Should().BeNull();
            createOrderResult.Data.CreateOrder.Errors.Should().HaveCountGreaterThan(0);
        }

        [Then(@"an appropriate error message should be returned")]
        public void ThenAnAppropriateErrorMessageShouldBeReturned()
        {
            var createOrderResult = _context.Get<IOperationResult<ICreateOrderResult>>("createOrderResult");
            createOrderResult.Data.Should().NotBeNull();
            createOrderResult.Data!.CreateOrder.Errors.Should().HaveCountGreaterThan(0);
        }
    }
}
