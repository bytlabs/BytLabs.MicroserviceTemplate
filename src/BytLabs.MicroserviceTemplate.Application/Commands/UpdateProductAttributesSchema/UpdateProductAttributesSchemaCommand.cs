using BytLabs.Application.CQS.Commands;
using BytLabs.MicroserviceTemplate.Application.Dtos;
using BytLabs.MicroserviceTemplate.Domain.Shared.DynamicData;

namespace BytLabs.MicroserviceTemplate.Application.Commands.UpdateProductAttributesSchema
{
    // Uses the FormDataSchema value object directly as the GraphQL input (mirrors
    // CandidateManagement). HotChocolate generates a single shared `FormDataSchemaInput`/
    // `DataSchemaInput` from the value objects, reused by every command that references them.
    public record UpdateProductAttributesSchemaCommand(Guid Id, FormDataSchema Schema) : ICommand<ProductDto>;
}
