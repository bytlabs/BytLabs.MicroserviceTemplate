using BytLabs.Domain.ValueObjects;

namespace BytLabs.MicroserviceTemplate.Domain.Common.DynamicData
{
    // RECIPE: Composite value object describing a dynamic table view (columns/filter/details).
    public class TableDataSchema : ValueObject
    {
        public TableDataSchema(DataSchema sampleData, DataSchema columns, DataSchema filter, DataSchema details)
        {
            Columns = columns;
            Filter = filter;
            SampleData = sampleData;
            Details = details;
        }

        public DataSchema SampleData { get; private set; }
        public DataSchema Columns { get; private set; }
        public DataSchema Filter { get; private set; }
        public DataSchema Details { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            return [SampleData, Columns, Filter, Details];
        }
    }
}
