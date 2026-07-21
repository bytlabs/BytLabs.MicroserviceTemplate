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
        [UseFiltering]
        public IExecutable<OrderDto> GetOrders(
            [Service] IMongoDatabase db,
            IResolverContext context,
            List<SortInput<OrderDto>>? order,
            CancellationToken cancellationToken)
        {
            // Dynamic-data `data` filtering is applied by MongoDynamicDataFilterFieldHandler via the
            // filter middleware (not inline). Sorting stays inline.
            return db.GetCollection<Order>()
                     .Aggregate()
                     .ExcludeSoftDeletedEntites()
                     .Project(Builders<Order>.Projection.As<OrderDto>())
                     .AppySortingWithDynamicData(order)
                     .AsExecutable();
        }
    }
}
