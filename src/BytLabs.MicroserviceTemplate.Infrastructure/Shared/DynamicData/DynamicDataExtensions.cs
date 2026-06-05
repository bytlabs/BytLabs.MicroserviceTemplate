using BytLabs.MicroserviceTemplate.Domain.Shared.DynamicData;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace BytLabs.MicroserviceTemplate.Infrastructure.Shared.DynamicData
{
    // RECIPE: register value objects with the BSON serializer (creators + defaults), so they
    // deserialize correctly even when fields are missing on older documents.
    public static class DynamicDataExtensions
    {
        public static IServiceCollection RegisterDynamicDataClassMaps(this IServiceCollection services)
        {
            BsonClassMap.TryRegisterClassMap<FormDataSchema>(cm =>
            {
                cm.AutoMap();
                cm.MapMember(c => c.Key).SetDefaultValue(() => string.Empty);
                cm.MapMember(c => c.SampleData).SetDefaultValue(() => new DataSchema(string.Empty, string.Empty));
                cm.MapMember(c => c.FormSchema).SetDefaultValue(() => new DataSchema(string.Empty, string.Empty));
                cm.MapMember(c => c.FormUi).SetDefaultValue(() => new DataSchema(string.Empty, string.Empty));
                cm.MapCreator(value => new FormDataSchema(value.Key, value.SampleData, value.FormSchema, value.FormUi));
            });

            BsonClassMap.TryRegisterClassMap<TableDataSchema>(cm =>
            {
                cm.AutoMap();
                cm.MapMember(c => c.Columns).SetDefaultValue(() => new DataSchema(string.Empty, string.Empty));
                cm.MapMember(c => c.Filter).SetDefaultValue(() => new DataSchema(string.Empty, string.Empty));
                cm.MapMember(c => c.SampleData).SetDefaultValue(() => new DataSchema(string.Empty, string.Empty));
                cm.MapMember(c => c.Details).SetDefaultValue(() => new DataSchema(string.Empty, string.Empty));
                cm.MapCreator(value => new TableDataSchema(value.SampleData, value.Columns, value.Filter, value.Details));
            });

            BsonClassMap.TryRegisterClassMap<DataSchema>(cm =>
            {
                cm.AutoMap();
                cm.MapMember(c => c.Type).SetDefaultValue(() => string.Empty);
                cm.MapMember(c => c.Data).SetDefaultValue(() => string.Empty);
                cm.MapCreator(value => new DataSchema(value.Type, value.Data));
            });

            return services;
        }
    }
}
