using System.Globalization;
using BytLabs.Application.DynamicData;
using BytLabs.Domain.DynamicData;
using HotChocolate.Configuration;
using HotChocolate.Data.Filters;
using HotChocolate.Data.MongoDb;
using HotChocolate.Data.MongoDb.Filters;
using HotChocolate.Language;
using HotChocolate.Language.Visitors;
using HotChocolate.Types;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using FilterOperation = BytLabs.Application.DynamicData.FilterOperation;
using ValueKind = BytLabs.Application.DynamicData.ValueKind;

namespace BytLabs.MicroserviceTemplate.Api.HotChocolate;

/// <summary>
/// MongoDB equivalent of <see cref="QueryableDynamicDataFilterFieldHandler"/>: translates the custom
/// dynamic-data <c>data</c> filter field into a <c>data.&lt;path&gt;</c> MongoDB filter, mirroring the
/// package's <c>FilterDataField</c>. Registered on the MongoDB filter provider.
/// </summary>
public sealed class MongoDynamicDataFilterFieldHandler
    : FilterFieldHandler<MongoDbFilterVisitorContext, MongoDbFilterDefinition>
{
    private readonly InputParser _inputParser;

    public MongoDynamicDataFilterFieldHandler(InputParser inputParser) => _inputParser = inputParser;

    public override bool CanHandle(
        ITypeCompletionContext context,
        IFilterInputTypeConfiguration typeConfiguration,
        IFilterFieldConfiguration fieldConfiguration)
        => typeof(IHaveDynamicData).IsAssignableFrom(typeConfiguration.EntityType)
           && string.Equals(fieldConfiguration.Name, "data", StringComparison.Ordinal);

    public override bool TryHandleEnter(
        MongoDbFilterVisitorContext context,
        IFilterField field,
        ObjectFieldNode node,
        out ISyntaxVisitorAction action)
    {
        if (node.Value.Kind != SyntaxKind.NullValue &&
            _inputParser.ParseLiteral(node.Value, field.Type, null) is DataOperationFilter op &&
            BuildBson(op) is { } bson)
        {
            context.GetLevel().Enqueue(new BsonMongoDbFilterDefinition(bson));
        }

        action = SyntaxVisitor.SkipAndLeave;
        return true;
    }

    private static BsonDocument? BuildBson(DataOperationFilter op)
    {
        if (string.IsNullOrWhiteSpace(op.Path)) return null;

        var fieldPath = $"data.{op.Path}";

        if (op.Operation == FilterOperation.Contains)
            return new BsonDocument(fieldPath, new BsonDocument("$regex", op.Value));

        var value = ToBson(op);
        return op.Operation switch
        {
            FilterOperation.Eq => new BsonDocument(fieldPath, value),
            FilterOperation.Ne => new BsonDocument(fieldPath, new BsonDocument("$ne", value)),
            FilterOperation.Gt => new BsonDocument(fieldPath, new BsonDocument("$gt", value)),
            FilterOperation.Lt => new BsonDocument(fieldPath, new BsonDocument("$lt", value)),
            FilterOperation.Gte => new BsonDocument(fieldPath, new BsonDocument("$gte", value)),
            FilterOperation.Lte => new BsonDocument(fieldPath, new BsonDocument("$lte", value)),
            _ => null,
        };
    }

    private static BsonValue ToBson(DataOperationFilter op) => op.ValueType switch
    {
        ValueKind.Number => double.TryParse(op.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var d)
            ? new BsonDouble(d) : BsonNull.Value,
        ValueKind.Boolean => bool.TryParse(op.Value, out var b) ? BsonBoolean.Create(b) : BsonNull.Value,
        ValueKind.DateTime => DateTime.TryParse(op.Value, CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var dt)
            ? new BsonDateTime(DateTime.SpecifyKind(dt, DateTimeKind.Utc)) : BsonNull.Value,
        _ => new BsonString(op.Value),
    };
}

/// <summary>A <see cref="MongoDbFilterDefinition"/> that renders a pre-built <see cref="BsonDocument"/>.</summary>
internal sealed class BsonMongoDbFilterDefinition : MongoDbFilterDefinition
{
    private readonly BsonDocument _document;

    public BsonMongoDbFilterDefinition(BsonDocument document) => _document = document;

    public override BsonDocument Render(IBsonSerializer documentSerializer, IBsonSerializerRegistry serializerRegistry)
        => _document;
}
