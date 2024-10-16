using HotChocolate.Execution.Configuration;
using BytLabs.Api.Graphql;
using BytLabs.MicroserviceTemplate.Application.Commands.CreateOrder;
using BytLabs.MicroserviceTemplate.Application.Commands.ShipOrder;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate;

namespace BytLabs.MicroserviceTemplate.Infrastructure
{
    public static class RequestExecutorBuilderExtensions
    {

        public static IRequestExecutorBuilder AddCommandTypes(this IRequestExecutorBuilder requestExecutorBuilder)
        {
            return requestExecutorBuilder
                .AddCommandType<CreateOrderCommand>()
                .AddCommandType<ShipOrderCommand>();
        }

        public static IRequestExecutorBuilder AddAggregateTypes(this IRequestExecutorBuilder requestExecutorBuilder)
        {
            return requestExecutorBuilder
                .AddAggregateType<Order, Guid>();
        }
    }
}
