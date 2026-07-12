using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace BytLabs.MicroserviceTemplate.Infrastructure.MongoDB
{
    // RECIPE: Custom BSON serializer so IReadOnlySet<T> round-trips as a BSON array.
    public class IReadOnlySetSerializer<T> : SerializerBase<IReadOnlySet<T>>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, IReadOnlySet<T> value)
        {
            context.Writer.WriteStartArray();
            foreach (var item in value)
            {
                BsonSerializer.Serialize(context.Writer, item);
            }
            context.Writer.WriteEndArray();
        }

        public override IReadOnlySet<T> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var data = BsonSerializer.Deserialize<IEnumerable<T>>(context.Reader);
            return data.ToHashSet();
        }
    }
}
