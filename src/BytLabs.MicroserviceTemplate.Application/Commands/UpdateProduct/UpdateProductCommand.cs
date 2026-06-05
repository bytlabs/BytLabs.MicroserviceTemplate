using System.Text.Json;
using BytLabs.Application.CQS.Commands;
using BytLabs.MicroserviceTemplate.Application.Dtos;

namespace BytLabs.MicroserviceTemplate.Application.Commands.UpdateProduct
{
    public record UpdateProductCommand(Guid Id, string Name, JsonElement Data) : ICommand<ProductDto>;
}
