using System.Text.Json;
using BytLabs.MicroserviceTemplate.Application.Products.Commands.AddVariant;
using BytLabs.MicroserviceTemplate.Application.Products.Commands.CreateProduct;
using BytLabs.MicroserviceTemplate.Application.Products.Commands.RemoveProduct;
using BytLabs.MicroserviceTemplate.Application.Products.Commands.RemoveVariant;
using BytLabs.MicroserviceTemplate.Application.Products.Commands.UpdateProduct;
using BytLabs.MicroserviceTemplate.Domain.Products.Aggregates;
using BytLabs.MicroserviceTemplate.Domain.Products.Inputs;
using BytLabs.MicroserviceTemplate.Api.OData.Resources;
using BytLabs.MicroserviceTemplate.Api.Querying;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace BytLabs.MicroserviceTemplate.Api.Controllers;

public class ProductsController : ODataController
{
    private readonly IMediator _mediator;
    private readonly IQueryable<Product> _products;

    public ProductsController(IMediator mediator, IQueryable<Product> products)
    {
        _mediator = mediator;
        _products = products.ExcludeSoftDeletedEntities();
    }

    [EnableQuery]
    public ActionResult<IEnumerable<ProductResource>> Get()
        => Ok(_products.Select(Map));

    [EnableQuery]
    public ActionResult<ProductResource> Get([FromRoute] Guid key)
    {
        var product = _products.Where(p => p.Id == key).FirstOrDefault();
        return product is null ? NotFound() : Ok(Map(product));
    }

    public async Task<IActionResult> Post([FromBody] ProductResource resource, CancellationToken ct)
    {
        var command = new CreateProductCommand(
            resource.Id == Guid.Empty ? Guid.NewGuid() : resource.Id,
            resource.Name,
            ParseData(resource.Data),
            resource.Variants.Select(v => new VariantData(v.Sku, v.Price)));

        var dto = await _mediator.Send(command, ct);
        var created = _products.First(p => p.Id == dto.Id);
        return Created(Map(created));
    }

    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] ProductResource resource, CancellationToken ct)
    {
        var command = new UpdateProductCommand(
            key,
            resource.Name,
            ParseData(resource.Data),
            resource.Variants.Select(v => new VariantData(v.Sku, v.Price)));

        var dto = await _mediator.Send(command, ct);
        var updated = _products.First(p => p.Id == dto.Id);
        return Updated(Map(updated));
    }

    public async Task<IActionResult> Delete([FromRoute] Guid key, CancellationToken ct)
    {
        await _mediator.Send(new RemoveProductCommand(key), ct);
        return NoContent();
    }

    [HttpPost("odata/Products({key})/AddVariant")]
    public async Task<IActionResult> AddVariant([FromRoute] Guid key, [FromBody] ProductVariantResource variant, CancellationToken ct)
    {
        await _mediator.Send(new AddVariantCommand(
            key,
            variant.Id == Guid.Empty ? Guid.NewGuid() : variant.Id,
            variant.Sku,
            variant.Price), ct);

        var updated = _products.First(p => p.Id == key);
        return Ok(Map(updated));
    }

    [HttpPost("odata/Products({key})/RemoveVariant")]
    public async Task<IActionResult> RemoveVariant([FromRoute] Guid key, [FromBody] ProductVariantResource variant, CancellationToken ct)
    {
        await _mediator.Send(new RemoveVariantCommand(key, variant.Id), ct);
        var updated = _products.First(p => p.Id == key);
        return Ok(Map(updated));
    }

    private static JsonElement ParseData(string? data)
        => string.IsNullOrWhiteSpace(data)
            ? JsonDocument.Parse("{}").RootElement.Clone()
            : JsonDocument.Parse(data).RootElement.Clone();

    private static ProductResource Map(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Data = p.Data.ValueKind == JsonValueKind.Undefined ? "{}" : p.Data.GetRawText(),
        Variants = p.Variants.Select(v => new ProductVariantResource
        {
            Id = v.Id, Sku = v.Sku, Price = v.Price
        }).ToList(),
        CreatedAt = p.AuditInfo?.CreatedAt,
        CreatedBy = p.AuditInfo?.CreatedBy
    };
}
