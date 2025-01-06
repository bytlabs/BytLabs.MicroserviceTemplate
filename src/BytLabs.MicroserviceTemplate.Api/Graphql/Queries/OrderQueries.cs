using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate;
using MongoDB.Driver;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate;
using BytLabs.DataAccess.MongoDB.Extensions;

namespace BytLabs.MicroserviceTemplate.Api.Graphql.Queries
{
    public partial class Query
    {
        [UsePaging]
        [UseProjection]
        [UseSorting]
        [UseFiltering]
        public IExecutable<Order> GetOrders([Service] IMongoDatabase db, CancellationToken cancellationToken)
        {
            return db.GetCollection<Order>().AsExecutable();
        }

    }
}
