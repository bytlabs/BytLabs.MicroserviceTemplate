using BytLabs.MicroserviceTemplate.Application.Products.Dtos;
using BytLabs.MicroserviceTemplate.Domain.Products.Aggregates;
using BytLabs.MicroserviceTemplate.Domain.Products.Entities;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate;
using MongoDB.Driver;
using BytLabs.DataAccess.MongoDB.Extensions;
using HotChocolate.Resolvers;
using BytLabs.MicroserviceTemplate.Api.Utils;
using BytLabs.DataAccess.MongoDB.DynamicData;
using BytLabs.Application.DynamicData;
using HotChocolate.Authorization;

namespace BytLabs.MicroserviceTemplate.Api.Graphql.Queries.Mongo
{
    public partial class Query
    {
        // RECIPE: advanced query — excludes soft-deleted rows, supports dynamic-data `where`
        // filtering and sorting, projects to a DTO, and is protected by [Authorize]
        // (method-level so the simple Order query stays open).
        [Authorize]
        [UsePaging]
        [UseProjection]
        [UseFiltering(Type = typeof(Product))]
        public IExecutable<ProductDto> GetProducts(
            [Service] IMongoDatabase db,
            IResolverContext context,
            List<SortInput<Product>>? order,
            CancellationToken cancellationToken)
        {
            return db.GetCollection<Product>()
                     .Aggregate()
                     .ExcludeSoftDeletedEntites()
                     .ApplyDynamicDataFilteration(context)
                     .AppySortingWithDynamicData(order)
                     .Project(Builders<Product>.Projection.As<ProductDto>())
                     .AsExecutable();
        }
    }
}
