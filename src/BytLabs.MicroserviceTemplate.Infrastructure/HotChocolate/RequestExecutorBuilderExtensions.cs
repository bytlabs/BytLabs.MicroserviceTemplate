using HotChocolate.Execution.Configuration;
using HotChocolate.Data.Filters;
using BytLabs.Api.Graphql;
using BytLabs.MicroserviceTemplate.Application.Orders.Commands.CreateOrder;
using BytLabs.MicroserviceTemplate.Application.Orders.Commands.ShipOrder;
using BytLabs.MicroserviceTemplate.Application.Orders.Commands.UpdateOrder;
using BytLabs.MicroserviceTemplate.Application.Orders.Commands.RemoveOrder;
using BytLabs.MicroserviceTemplate.Application.Products.Commands.CreateProduct;
using BytLabs.MicroserviceTemplate.Application.Products.Commands.UpdateProduct;
using BytLabs.MicroserviceTemplate.Application.Products.Commands.RemoveProduct;
using BytLabs.MicroserviceTemplate.Application.Products.Commands.AddVariant;
using BytLabs.MicroserviceTemplate.Application.Products.Commands.RemoveVariant;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Commands.CreateEntityDef;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Commands.UpdateEntityDef;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Commands.RemoveEntityDef;
using Microsoft.Extensions.DependencyInjection;
using BytLabs.MicroserviceTemplate.Application.Orders.Dtos;
using BytLabs.MicroserviceTemplate.Application.Products.Dtos;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Dtos;
using BytLabs.MicroserviceTemplate.Domain.Orders;
using BytLabs.MicroserviceTemplate.Domain.Products;
using BytLabs.MicroserviceTemplate.Domain.EntityDefs;

namespace BytLabs.MicroserviceTemplate.Infrastructure.HotChocolate
{
    public static class RequestExecutorBuilderExtensions
    {
        public static IRequestExecutorBuilder AddCommandTypes(this IRequestExecutorBuilder requestExecutorBuilder)
        {
            return requestExecutorBuilder
                .AddCommandType<CreateOrderCommand>()
                .AddCommandType<ShipOrderCommand>()
                .AddCommandType<UpdateOrderCommand>()
                .AddCommandType<RemoveOrderCommand>()
                .AddCommandType<CreateProductCommand>()
                .AddCommandType<UpdateProductCommand>()
                .AddCommandType<RemoveProductCommand>()
                .AddCommandType<AddVariantCommand>()
                .AddCommandType<RemoveVariantCommand>()
                .AddCommandType<CreateEntityDefCommand>()
                .AddCommandType<UpdateEntityDefCommand>()
                .AddCommandType<RemoveEntityDefCommand>();
        }

        public static IRequestExecutorBuilder AddDtoTypes(this IRequestExecutorBuilder requestExecutorBuilder)
        {
            return requestExecutorBuilder
                .AddDtoType<OrderDto>()
                .AddDtoType<OrderItemDto>()
                .AddType<ProductDtoType>()          // custom DtoType for Product
                .AddDtoType<ProductVariantDto>()
                .AddDtoType<EntityDefDto>();
        }

        public static IRequestExecutorBuilder AddAggregateTypes(this IRequestExecutorBuilder requestExecutorBuilder)
        {
            // Only the dynamic-data Product aggregate needs the BytLabs aggregate filter/sort
            // input types. Order uses HotChocolate's built-in [UseFiltering]/[UseSorting].
            return requestExecutorBuilder
                .AddAggregateSortType<Order, Guid>()
                .AddAggregateFilterType<Order, Guid>()
                .AddAggregateSortType<Product, Guid>()
                .AddAggregateFilterType<Product, Guid>()
                // EntityDef query uses [UseSorting(Type=typeof(EntityDef))] which generates the sort
                // input itself, so only the aggregate filter type is registered here (mirrors
                // CandidateManagement's OrganizationSubEntityDef). Registering the sort type too
                // would duplicate `EntityDefSortInput`.
                .AddAggregateFilterType<EntityDef, Guid>();
        }
    }
}
