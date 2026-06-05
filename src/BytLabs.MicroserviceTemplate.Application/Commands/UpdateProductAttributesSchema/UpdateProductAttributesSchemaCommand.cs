using BytLabs.Application.CQS.Commands;
using BytLabs.MicroserviceTemplate.Application.Dtos;

namespace BytLabs.MicroserviceTemplate.Application.Commands.UpdateProductAttributesSchema
{
    // GraphQL-friendly input records that map to the FormDataSchema/DataSchema value objects.
    public record DataSchemaInput(string Type, string Data);
    public record FormDataSchemaInput(string Key, DataSchemaInput SampleData, DataSchemaInput FormSchema, DataSchemaInput FormUi);

    public record UpdateProductAttributesSchemaCommand(Guid Id, FormDataSchemaInput Schema) : ICommand<ProductDto>;
}
