using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate;
using MongoDB.Driver;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate;
using BytLabs.DataAccess.MongoDB.Extensions;
using BytLabs.MicroserviceTemplate.Application.Dtos;

namespace BytLabs.MicroserviceTemplate.Api.Graphql.Queries
{
    public partial class Query
    {
        // Simple example: paged/filterable/sortable projection over the Order collection.
        [UsePaging]
        [UseProjection]
        [UseSorting(Type = typeof(Order))]
        [UseFiltering(Type = typeof(Order))]
        public IExecutable<OrderDto> GetOrders([Service] IMongoDatabase db)
        {
            return db.GetCollection<Order>()
                     .Aggregate()
                     .Project(Builders<Order>.Projection.As<OrderDto>())
                     .AsExecutable();
        }
    }
}
