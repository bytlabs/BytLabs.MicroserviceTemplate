using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate;
using HotChocolate.Resolvers;
using MongoDB.Driver;
using BytLabs.MicroserviceTemplate.Domain.Orders.Aggregates;
using BytLabs.MicroserviceTemplate.Domain.Orders.Entities;
using BytLabs.MicroserviceTemplate.Domain.Orders.ValueObjects;
using BytLabs.DataAccess.MongoDB.Extensions;
using BytLabs.DataAccess.MongoDB.DynamicData;
using BytLabs.Application.DynamicData;
using BytLabs.MicroserviceTemplate.Application.Orders.Dtos;
using BytLabs.MicroserviceTemplate.Api.HotChocolate;
using BytLabs.DataAccess.MongoDB;

namespace BytLabs.MicroserviceTemplate.Api.Graphql.Queries.Mongo
{
    public partial class MongoQuery
    {
        // Order is the "open" dynamic entity (no [Authorize], unlike Product): excludes soft-deleted
        // rows and supports dynamic-data filtering/sorting so the console can browse it out of the box.
        [UsePaging]
        [UseProjection]
        [UseFiltering(Type = typeof(Order))]
        public IExecutable<OrderDto> GetOrders(
            [Service] IMongoDatabase db,
            IResolverContext context,
            List<SortInput<Order>>? order,
            CancellationToken cancellationToken)
        {
            return db.GetCollection<Order>()
                     .Aggregate()
                     .ExcludeSoftDeletedEntites()
                     .ApplyDynamicDataFilteration(context)
                     .AppySortingWithDynamicData(order)
                     .Project(Builders<Order>.Projection.As<OrderDto>())
                     .AsExecutable();
        }
    }
}
