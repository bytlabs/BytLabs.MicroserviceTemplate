using System.Text.Json;
using BytLabs.Application.CQS.Commands;
using BytLabs.MicroserviceTemplate.Application.Dtos;

namespace BytLabs.MicroserviceTemplate.Application.Commands.CreateProduct
{
    public record CreateProductCommand(Guid Id, string Name, JsonElement Data) : ICommand<ProductDto>;
}
