using BytLabs.Application.DynamicData;
using BytLabs.DataAccess.MongoDB.DynamicData;
using BytLabs.Domain.DynamicData;
using BytLabs.MicroserviceTemplate.Api.HotChocolate;
using HotChocolate.Resolvers;
using MongoDB.Driver;
using BytLabs.MicroserviceTemplate.Infrastructure.Postgres.DynamicData;

namespace BytLabs.MicroserviceTemplate.Api.HotChocolate
{
    public static class IDynamicDataFilterExtensions
    {
        public static IQueryable<T> ApplyDynamicDataFilteration<T>(this IQueryable<T> queryable, IResolverContext context)
           where T : class, IHaveDynamicData
        {
            var dataFilter = context.ArgumentValue<InputFilteringDynamicData?>("where");
            return queryable.ApplyDynamicDataFilteration(dataFilter);
        }

        // MongoDB aggregate path: read the dynamic-data `where` from the resolver and apply it as a
        // $match before the projection (the MongoQuery resolvers use this).
        public static IAggregateFluent<T> ApplyDynamicDataFilteration<T>(this IAggregateFluent<T> aggregate, IResolverContext context)
           where T : IHaveDynamicData
        {
            var dataFilter = context.ArgumentValue<InputFilteringDynamicData?>("where");
            return dataFilter is null ? aggregate : aggregate.ApplyDynamicDataFilteration(dataFilter);
        }
    }
}
