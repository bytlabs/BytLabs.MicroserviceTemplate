using System.Linq.Expressions;
using BytLabs.Domain.DynamicData;
using HotChocolate.Data.Filters;
using HotChocolate.Data.Filters.Expressions;
using HotChocolate.Configuration;
using HotChocolate.Language;
using HotChocolate.Language.Visitors;
using HotChocolate.Types.Descriptors.Definitions;

namespace BytLabs.MicroserviceTemplate.Api.Graphql.Filtering;

/// <summary>
/// Lets the queryable (EF) filter provider accept the custom dynamic-data <c>data</c> field on
/// <c>*FilterInput</c> types. The field's type is a plain <c>InputObjectType&lt;DataOperationFilter&gt;</c>
/// (not a FilterInputType), so no built-in queryable handler matches it and schema build fails with
/// "no handler found". This handler matches only that field and skips it — the resolver applies the
/// dynamic-data (jsonb) predicate manually (see JsonbDynamicDataFilter), mirroring the MongoDB path.
/// </summary>
public sealed class DataPassThroughFilterFieldHandler : FilterFieldHandler<QueryableFilterContext, Expression>
{
    public override bool CanHandle(
        ITypeCompletionContext context,
        IFilterInputTypeDefinition typeDefinition,
        IFilterFieldDefinition fieldDefinition)
        => typeof(IHaveDynamicData).IsAssignableFrom(typeDefinition.EntityType)
           && string.Equals(fieldDefinition.Name, "data", StringComparison.Ordinal);

    public override bool TryHandleEnter(
        QueryableFilterContext context,
        IFilterField field,
        ObjectFieldNode node,
        out ISyntaxVisitorAction action)
    {
        // Don't descend into path/operation/value and don't invoke Leave; contribute no expression.
        action = SyntaxVisitor.SkipAndLeave;
        return true;
    }

    public override bool TryHandleLeave(
        QueryableFilterContext context,
        IFilterField field,
        ObjectFieldNode node,
        out ISyntaxVisitorAction action)
    {
        action = SyntaxVisitor.Continue;
        return true;
    }
}
