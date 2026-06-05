using BytLabs.Application.DynamicData;
using BytLabs.DataAccess.MongoDB.DynamicData;
using BytLabs.Domain.DynamicData;
using HotChocolate.Resolvers;
using MongoDB.Driver;

namespace BytLabs.MicroserviceTemplate.Api.Utils
{
    public static class IAggregateFluentExtensions
    {
        // RECIPE: apply GraphQL `where` filtering over the dynamic JSON `Data` field.
        public static IAggregateFluent<T> ApplyDynamicDataFilteration<T>(this IAggregateFluent<T> aggregateFluent, IResolverContext context)
            where T : IHaveDynamicData
        {
            var dataFilter = context.ArgumentValue<InputFilteringDynamicData?>("where");
            return aggregateFluent.ApplyDynamicDataFilteration(dataFilter);
        }
    }
}
