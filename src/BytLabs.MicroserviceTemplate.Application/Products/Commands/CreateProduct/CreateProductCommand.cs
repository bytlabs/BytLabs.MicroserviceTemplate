using System.Text.Json;
using BytLabs.Application.CQS.Commands;
using BytLabs.MicroserviceTemplate.Application.Products.Dtos;

namespace BytLabs.MicroserviceTemplate.Application.Products.Commands.CreateProduct
{
    public record CreateProductCommand(Guid Id, string Name, JsonElement Data) : ICommand<ProductDto>;
}
