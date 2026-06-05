using BytLabs.Domain.ValueObjects;

namespace BytLabs.MicroserviceTemplate.Domain.Shared.DynamicData
{
    // RECIPE: Value Object. A DataSchema is an immutable pair describing a piece of
    // dynamic/UI schema (e.g. a JSON-schema fragment + its UI hints). Equality is by value.
    public class DataSchema : ValueObject
    {
        public string Type { get; private set; }
        public string Data { get; private set; }

        public DataSchema(string type, string data)
        {
            Type = type;
            Data = data;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            return [Type, Data];
        }
    }
}
