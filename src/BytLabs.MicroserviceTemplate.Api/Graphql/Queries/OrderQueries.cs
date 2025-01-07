using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate;
using MongoDB.Driver;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate;
using BytLabs.DataAccess.MongoDB.Extensions;
using AutoMapper;
using BytLabs.MicroserviceTemplate.Application.Dtos;
using AutoMapper.QueryableExtensions;
using BytLabs.Api.Graphql;

namespace BytLabs.MicroserviceTemplate.Api.Graphql.Queries
{
    public partial class Query
    {

        [UsePaging]
        [UseProjection]
        [UseSorting(Type = typeof(Order))]
        [UseFiltering(Type = typeof(Order))]
        public IExecutable<OrderDto> GetOrders(
            [Service] IMongoDatabase db, 
            [Service] IMapper mapper,
            CancellationToken cancellationToken)
        {
            return db.GetCollection<Order>()
                .AsQueryable()
                .ProjectTo<OrderDto>(mapper.ConfigurationProvider)
                .AsExecutable();
        }

    }
}
