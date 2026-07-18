using System.Linq.Expressions;
using BytLabs.Application.DynamicData;
using BytLabs.Domain.DynamicData;
using BytLabs.MicroserviceTemplate.Infrastructure.Postgres.DynamicData;
using HotChocolate.Configuration;
using HotChocolate.Data.Filters;
using HotChocolate.Data.Filters.Expressions;
using HotChocolate.Language;
using HotChocolate.Language.Visitors;
using HotChocolate.Types;

namespace BytLabs.MicroserviceTemplate.Api.HotChocolate;

/// <summary>
/// Translates the custom dynamic-data <c>data</c> filter field (<see cref="DataOperationFilter"/>) into an
/// EF-translatable jsonb predicate on the queryable (IQueryable/EF) filter provider. Replaces the manual
/// resolver-side application: HotChocolate's filter middleware now applies it, composing with the built-in
/// <c>and</c>/<c>or</c> handlers and paging. Registered on the queryable filter provider.
/// </summary>
public sealed class QueryableDynamicDataFilterFieldHandler
    : FilterFieldHandler<QueryableFilterContext, Expression>
{
    private readonly InputParser _inputParser;

    public QueryableDynamicDataFilterFieldHandler(InputParser inputParser) => _inputParser = inputParser;

    public override bool CanHandle(
        ITypeCompletionContext context,
        IFilterInputTypeConfiguration typeConfiguration,
        IFilterFieldConfiguration fieldConfiguration)
        => typeof(IHaveDynamicData).IsAssignableFrom(typeConfiguration.EntityType)
           && string.Equals(fieldConfiguration.Name, "data", StringComparison.Ordinal);

    public override bool TryHandleEnter(
        QueryableFilterContext context,
        IFilterField field,
        ObjectFieldNode node,
        out ISyntaxVisitorAction action)
    {
        // `data: null` contributes nothing.
        if (node.Value.Kind != SyntaxKind.NullValue &&
            _inputParser.ParseLiteral(node.Value, field.Type, null) is DataOperationFilter op)
        {
            var predicate = JsonbDynamicDataFilter.BuildLeaf(op, context.GetInstance());
            if (predicate is not null)
                context.GetLevel().Enqueue(predicate);
        }

        // We consumed the whole DataOperationFilter object; don't descend into its sub-fields.
        action = SyntaxVisitor.SkipAndLeave;
        return true;
    }
}
