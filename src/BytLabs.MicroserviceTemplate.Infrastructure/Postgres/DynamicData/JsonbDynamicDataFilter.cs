using System.Globalization;
using System.Linq.Expressions;
using System.Text.Json;
using BytLabs.Application.DynamicData;
using BytLabs.Domain.DynamicData;

namespace BytLabs.MicroserviceTemplate.Infrastructure.Postgres.DynamicData;

/// <summary>
/// Translates the dynamic-data GraphQL <c>where</c> (<see cref="InputFilteringDynamicData"/>) into an
/// EF-translatable predicate over the aggregate's <c>Data</c> jsonb column, mirroring the MongoDB
/// <c>FilterData</c>/<c>FilterDataField</c> semantics. Npgsql translates
/// <c>e.Data.GetProperty("x").GetString()</c> to <c>data-&gt;&gt;'x'</c> (with a CAST for typed values),
/// so filtering pushes down to SQL.
/// </summary>
public static class JsonbDynamicDataFilter
{
    private static readonly System.Reflection.MethodInfo GetPropertyMethod =
        typeof(JsonElement).GetMethod(nameof(JsonElement.GetProperty), new[] { typeof(string) })!;

    /// <summary>
    /// Applies <see cref="SortInput{T}"/> ordering. A <c>Path</c> of a CLR property sorts that column;
    /// a <c>data.&lt;key&gt;</c> path (or an unmatched name) sorts by the jsonb sub-value. Mirrors the
    /// MongoDB <c>AppySortingWithDynamicData</c>.
    /// </summary>
    public static IQueryable<T> ApplyDynamicDataSorting<T>(this IQueryable<T> source, List<SortInput<T>>? order)
        where T : class, IHaveDynamicData
    {
        if (order is null || order.Count == 0) return source;

        var q = source;
        var first = true;
        foreach (var sort in order)
        {
            if (string.IsNullOrWhiteSpace(sort.Path)) continue;
            var param = Expression.Parameter(typeof(T), "e");
            var key = BuildSortKey<T>(sort.Path, param);
            var lambda = Expression.Lambda(key, param);
            var ascending = sort.By == SortOrder.Asc;
            var method = first
                ? (ascending ? "OrderBy" : "OrderByDescending")
                : (ascending ? "ThenBy" : "ThenByDescending");
            q = q.Provider.CreateQuery<T>(Expression.Call(
                typeof(Queryable), method, new[] { typeof(T), key.Type }, q.Expression, Expression.Quote(lambda)));
            first = false;
        }
        return q;
    }

    private static Expression BuildSortKey<T>(string path, ParameterExpression param) where T : IHaveDynamicData
    {
        // Explicit data path.
        if (path.StartsWith("data.", StringComparison.OrdinalIgnoreCase))
            return JsonPathText(param, path[5..].Split('.', StringSplitOptions.RemoveEmptyEntries));

        // Matching CLR property (case-insensitive) sorts the column.
        var prop = typeof(T).GetProperty(path,
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
        if (prop is not null) return Expression.Property(param, prop);

        // Otherwise treat as a top-level dynamic-data key.
        return JsonPathText(param, new[] { path });
    }

    private static Expression JsonPathText(ParameterExpression param, string[] segments)
    {
        Expression element = Expression.Property(param, nameof(IHaveDynamicData.Data));
        foreach (var segment in segments)
            element = Expression.Call(element, GetPropertyMethod, Expression.Constant(segment));
        return Expression.Call(element, nameof(JsonElement.GetString), Type.EmptyTypes);
    }

    public static IQueryable<T> ApplyDynamicDataFilteration<T>(this IQueryable<T> source, InputFilteringDynamicData? filter)
        where T : class, IHaveDynamicData
    {
        if (filter is null) return source;
        var param = Expression.Parameter(typeof(T), "e");
        var body = BuildNode<T>(filter, param);
        return body is null ? source : source.Where(Expression.Lambda<Func<T, bool>>(body, param));
    }

    // Mirrors FilterData: AND together the (recursive) And branch, the (recursive) Or branch, and the leaf Data predicate.
    private static Expression? BuildNode<T>(InputFilteringDynamicData node, ParameterExpression param)
        where T : IHaveDynamicData
    {
        var parts = new List<Expression>();

        if (node.And is not null)
        {
            var andParts = node.And.Select(a => BuildNode<T>(a, param)).Where(e => e is not null).Cast<Expression>().ToList();
            if (andParts.Count > 0) parts.Add(andParts.Aggregate(Expression.AndAlso));
        }

        if (node.Or is not null)
        {
            var orParts = node.Or.Select(o => BuildNode<T>(o, param)).Where(e => e is not null).Cast<Expression>().ToList();
            if (orParts.Count > 0) parts.Add(orParts.Aggregate(Expression.OrElse));
        }

        if (node.Data is not null)
        {
            var leaf = BuildLeaf(node.Data, param);
            if (leaf is not null) parts.Add(leaf);
        }

        return parts.Count == 0 ? null : parts.Aggregate(Expression.AndAlso);
    }

    private static Expression? BuildLeaf(DataOperationFilter op, ParameterExpression param)
    {
        if (string.IsNullOrWhiteSpace(op.Path)) return null;

        // e.Data, then GetProperty(segment) for each dotted path segment: data.a.b -> data->'a'->'b'
        Expression element = Expression.Property(param, nameof(IHaveDynamicData.Data));
        foreach (var segment in op.Path.Split('.', StringSplitOptions.RemoveEmptyEntries))
            element = Expression.Call(element, GetPropertyMethod, Expression.Constant(segment));

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
