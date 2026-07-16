using HotChocolate.Execution.Configuration;
using HotChocolate.Data;
using HotChocolate.Data.Filters;
using HotChocolate.Data.Filters.Expressions;
using BytLabs.MicroserviceTemplate.Api.Graphql.Filtering;

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

        // Provider-agnostic (IQueryable/EF) filtering/projection/sorting. Paging uses HotChocolate's
        // default queryable cursor provider. Used for the PostgreSQL GraphQL query root.
        public static IRequestExecutorBuilder AddQueryableQuerySettings(this IRequestExecutorBuilder builder)
        {
            return builder
                    .AddFiltering(d => d
                        .AddDefaults()
                        .AddProviderExtension(new QueryableFilterProviderExtension(x =>
                            x.AddFieldHandler<DataPassThroughFilterFieldHandler>())))
                    .AddProjections()
                    .AddSorting();
        }
    }
}
