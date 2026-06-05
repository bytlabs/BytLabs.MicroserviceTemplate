using HotChocolate.Execution.Configuration;

namespace BytLabs.MicroserviceTemplate.Api.Utils
{
    public static class IRequestExecutorBuilderExtensions
    {
        // RECIPE: enable MongoDB-aware filtering/projection/sorting/paging for GraphQL queries.
        public static IRequestExecutorBuilder AddMongoDbQuerySettings(this IRequestExecutorBuilder builder)
        {
            return builder
                    .AddMongoDbFiltering()
                    .AddMongoDbProjections()
                    .AddMongoDbSorting()
                    .AddMongoDbPagingProviders();
        }
    }
}
