using HotChocolate.Execution.Configuration;
using BytLabs.Api.Graphql;
using BytLabs.MicroserviceTemplate.Application.Commands.CreateOrder;
using BytLabs.MicroserviceTemplate.Application.Commands.ShipOrder;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate;
using Microsoft.Extensions.DependencyInjection;
using BytLabs.MicroserviceTemplate.Application.Dtos;
using HotChocolate.Types;
using static BytLabs.MicroserviceTemplate.Infrastructure.RequestExecutorBuilderExtensions;
using HotChocolate.Data.Filters;
using HotChocolate.Data.Sorting;

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

        public static IRequestExecutorBuilder AddDtoTypes(this IRequestExecutorBuilder requestExecutorBuilder)
        {
            return requestExecutorBuilder
                .AddDtoType<OrderDto>()
                .AddDtoType<OrderItemDto>();
        }
    }
}
