using BytLabs.Application.DynamicData;
using BytLabs.Domain.DynamicData;
using HotChocolate.Configuration;
using HotChocolate.Data.Filters;
using HotChocolate.Data.Filters.Expressions;
using HotChocolate.Language;
using HotChocolate.Language.Visitors;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using System.Globalization;
using System.Linq.Expressions;
using System.Text.Json;
using FilterOperation = BytLabs.Application.DynamicData.FilterOperation;
using ValueKind = BytLabs.Application.DynamicData.ValueKind;

namespace BytLabs.MicroserviceTemplate.Api.HotChocolate;


/// <summary>
/// Translates the custom dynamic-data <c>data</c> filter field (<see cref="DataOperationFilter"/>) into an
/// EF-translatable jsonb predicate on the queryable (IQueryable/EF) filter provider, so HotChocolate's
/// filter middleware applies it and composes with the built-in <c>and</c>/<c>or</c> handlers and paging.
/// Npgsql translates <c>e.Data.GetProperty("x").GetString()</c> to <c>data-&gt;&gt;'x'</c> (with a CAST for
/// typed values), so filtering pushes down to SQL.
/// </summary>
public sealed class QueryableDynamicDataFilterFieldHandler
    : FilterFieldHandler<QueryableFilterContext, Expression>
{
    private static readonly System.Reflection.MethodInfo s_getProperty =
        typeof(JsonElement).GetMethod(nameof(JsonElement.GetProperty), new[] { typeof(string) })!;

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
            _inputParser.ParseLiteral(node.Value, field.Type, null) is DataOperationFilter op &&
            BuildLeaf(op, context.GetInstance()) is { } predicate)
        {
            context.GetLevel().Enqueue(predicate);
        }

        // We consumed the whole DataOperationFilter object; don't descend into its sub-fields.
        action = SyntaxVisitor.SkipAndLeave;
        return true;
    }

    /// <summary>
    /// Builds the EF-translatable predicate for a single dynamic-data leaf against the given entity
    /// instance expression (the filter parameter <c>e</c>). Returns null for an empty path.
    /// </summary>
    private static Expression? BuildLeaf(DataOperationFilter op, Expression instance)
    {
        if (string.IsNullOrWhiteSpace(op.Path)) return null;

        // e.Data, then GetProperty(segment) for each dotted path segment: data.a.b -> data->'a'->'b'
        Expression element = Expression.Property(instance, nameof(IHaveDynamicData.Data));
        foreach (var segment in op.Path.Split('.', StringSplitOptions.RemoveEmptyEntries))
            element = Expression.Call(element, s_getProperty, Expression.Constant(segment));

        // Contains is string-only (matches Mongo's regex on the raw text).
        if (op.Operation == FilterOperation.Contains)
        {
            var getString = Expression.Call(element, nameof(JsonElement.GetString), Type.EmptyTypes);
            var contains = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;
            // GetString() can be null; guard so the translated SQL is null-safe.
            var notNull = Expression.NotEqual(getString, Expression.Constant(null, typeof(string)));
            var call = Expression.Call(getString, contains, Expression.Constant(op.Value));
            return Expression.AndAlso(notNull, call);
        }

        var (accessor, value) = op.ValueType switch
        {
            ValueKind.Number => (Call(element, nameof(JsonElement.GetDouble)),
                                  Const(double.Parse(op.Value, CultureInfo.InvariantCulture))),
            ValueKind.Boolean => (Call(element, nameof(JsonElement.GetBoolean)),
                                  Const(bool.Parse(op.Value))),
            ValueKind.DateTime => (Call(element, nameof(JsonElement.GetDateTime)),
                                   Const(DateTime.SpecifyKind(DateTime.Parse(op.Value, CultureInfo.InvariantCulture,
                                       DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal), DateTimeKind.Utc))),
            _ => (Call(element, nameof(JsonElement.GetString)), Const(op.Value)),
        };

        return op.Operation switch
        {
            FilterOperation.Eq => Expression.Equal(accessor, value),
            FilterOperation.Ne => Expression.NotEqual(accessor, value),
            FilterOperation.Gt => Expression.GreaterThan(accessor, value),
            FilterOperation.Lt => Expression.LessThan(accessor, value),
            FilterOperation.Gte => Expression.GreaterThanOrEqual(accessor, value),
            FilterOperation.Lte => Expression.LessThanOrEqual(accessor, value),
            _ => throw new NotSupportedException($"Unsupported dynamic-data filter operation '{op.Operation}'."),
        };
    }

    private static Expression Call(Expression element, string method)
        => Expression.Call(element, method, Type.EmptyTypes);

    private static Expression Const<TValue>(TValue value) => Expression.Constant(value, typeof(TValue));
}
