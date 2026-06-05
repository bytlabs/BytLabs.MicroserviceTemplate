using HotChocolate.Execution.Configuration;
using HotChocolate.Data.Filters;
using BytLabs.Api.Graphql;
using BytLabs.MicroserviceTemplate.Application.Commands.CreateOrder;
using BytLabs.MicroserviceTemplate.Application.Commands.ShipOrder;
using BytLabs.MicroserviceTemplate.Application.Commands.CreateProduct;
using BytLabs.MicroserviceTemplate.Application.Commands.UpdateProduct;
using BytLabs.MicroserviceTemplate.Application.Commands.RemoveProduct;
using BytLabs.MicroserviceTemplate.Application.Commands.UpdateProductAttributesSchema;
using BytLabs.MicroserviceTemplate.Application.Commands.AddVariant;
using BytLabs.MicroserviceTemplate.Application.Commands.RemoveVariant;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate;
using BytLabs.MicroserviceTemplate.Application.Dtos;
using BytLabs.MicroserviceTemplate.Infrastructure.DtoTypes;
using Microsoft.Extensions.DependencyInjection;

namespace BytLabs.MicroserviceTemplate.Infrastructure
{
    public static class RequestExecutorBuilderExtensions
    {
        public static IRequestExecutorBuilder AddCommandTypes(this IRequestExecutorBuilder requestExecutorBuilder)
        {
            return requestExecutorBuilder
                .AddCommandType<CreateOrderCommand>()
                .AddCommandType<ShipOrderCommand>()
                .AddCommandType<CreateProductCommand>()
                .AddCommandType<UpdateProductCommand>()
                .AddCommandType<RemoveProductCommand>()
                .AddCommandType<UpdateProductAttributesSchemaCommand>()
                .AddCommandType<AddVariantCommand>()
                .AddCommandType<RemoveVariantCommand>();
        }

        public static IRequestExecutorBuilder AddDtoTypes(this IRequestExecutorBuilder requestExecutorBuilder)
        {
            return requestExecutorBuilder
                .AddDtoType<OrderDto>()
                .AddDtoType<OrderItemDto>()
                .AddType<ProductDtoType>()          // custom DtoType for Product
                .AddDtoType<ProductVariantDto>();
        }

        public static IRequestExecutorBuilder AddAggregateTypes(this IRequestExecutorBuilder requestExecutorBuilder)
        {
            // Only the dynamic-data Product aggregate needs the BytLabs aggregate filter/sort
            // input types. Order uses HotChocolate's built-in [UseFiltering]/[UseSorting].
            return requestExecutorBuilder
                .AddAggregateSortType<Product, Guid>()
                .AddAggregateFilterType<Product, Guid>();
        }
    }
}
