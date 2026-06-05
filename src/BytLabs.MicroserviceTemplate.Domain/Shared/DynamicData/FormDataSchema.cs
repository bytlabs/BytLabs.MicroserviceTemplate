using BytLabs.Domain.ValueObjects;

namespace BytLabs.MicroserviceTemplate.Domain.Shared.DynamicData
{
    // RECIPE: Composite value object describing a dynamic form: a sample payload, the JSON schema,
    // and the UI schema, keyed by a logical name.
    public class FormDataSchema : ValueObject
    {
        public FormDataSchema(string key, DataSchema sampleData, DataSchema formSchema, DataSchema formUi)
        {
            Key = key;
            SampleData = sampleData;
            FormSchema = formSchema;
            FormUi = formUi;
        }

        public string Key { get; private set; }
        public DataSchema SampleData { get; private set; }
        public DataSchema FormSchema { get; private set; }
        public DataSchema FormUi { get; private set; }

        public static FormDataSchema Empty() => new(
            string.Empty,
            new DataSchema(string.Empty, string.Empty),
            new DataSchema(string.Empty, string.Empty),
            new DataSchema(string.Empty, string.Empty));

        protected override IEnumerable<object> GetEqualityComponents()
        {
            return [Key, SampleData, FormSchema, FormUi];
        }
    }
}
