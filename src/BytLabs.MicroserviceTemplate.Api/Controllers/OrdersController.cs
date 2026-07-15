using System.Text.Json;
using BytLabs.MicroserviceTemplate.Application.Orders.Commands.CreateOrder;
using BytLabs.MicroserviceTemplate.Application.Orders.Commands.RemoveOrder;
using BytLabs.MicroserviceTemplate.Application.Orders.Commands.ShipOrder;
using BytLabs.MicroserviceTemplate.Application.Orders.Commands.UpdateOrder;
using BytLabs.MicroserviceTemplate.Domain.Orders.Aggregates;
using BytLabs.MicroserviceTemplate.Domain.Orders.Entities;
using BytLabs.MicroserviceTemplate.Api.OData.Resources;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace BytLabs.MicroserviceTemplate.Api.Controllers;

public class OrdersController : ODataController
{
    private readonly IMediator _mediator;
    private readonly IQueryable<Order> _orders;

    public OrdersController(IMediator mediator, IQueryable<Order> orders)
    {
        _mediator = mediator;
        _orders = orders;
    }

    [EnableQuery]
    public ActionResult<IEnumerable<OrderResource>> Get()
        => Ok(_orders.ToList().Select(Map));

    [EnableQuery]
    public ActionResult<OrderResource> Get([FromRoute] Guid key)
    {
        var order = _orders.ToList().FirstOrDefault(o => o.Id == key);
        return order is null ? NotFound() : Ok(Map(order));
    }

    public async Task<IActionResult> Post([FromBody] OrderResource resource, CancellationToken ct)
    {
        var command = new CreateOrderCommand(
            resource.Id == Guid.Empty ? Guid.NewGuid() : resource.Id,
            resource.OrderDate,
            resource.Items.Select(i => new OrderItem(
                i.Id == Guid.Empty ? Guid.NewGuid() : i.Id, i.ProductId, i.Quantity, i.Price)),
            ParseData(resource.Data));

        var result = await _mediator.Send(command, ct);

        var created = _orders.ToList().First(o => o.Id == result.OrderId);
        return Created(Map(created));
    }

    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] OrderResource resource, CancellationToken ct)
    {
        var command = new UpdateOrderCommand(
            key,
            ParseData(resource.Data),
            resource.Items.Select(i => new OrderItem(
                i.Id == Guid.Empty ? Guid.NewGuid() : i.Id, i.ProductId, i.Quantity, i.Price)));

        var dto = await _mediator.Send(command, ct);
        var updated = _orders.ToList().First(o => o.Id == dto.Id);
        return Updated(Map(updated));
    }

    public async Task<IActionResult> Delete([FromRoute] Guid key, CancellationToken ct)
    {
        await _mediator.Send(new RemoveOrderCommand(key), ct);
        return NoContent();
    }

    [HttpPost("odata/Orders({key})/Ship")]
    public async Task<IActionResult> Ship([FromRoute] Guid key, CancellationToken ct)
    {
        await _mediator.Send(new ShipOrderCommand(key), ct);
        return NoContent();
    }

    private static JsonElement ParseData(string? data)
        => string.IsNullOrWhiteSpace(data)
            ? JsonDocument.Parse("{}").RootElement.Clone()
            : JsonDocument.Parse(data).RootElement.Clone();

    private static OrderResource Map(Order o) => new()
    {
        Id = o.Id,
        OrderDate = o.OrderDate,
        Status = o.Status.ToString(),
        Data = o.Data.ValueKind == JsonValueKind.Undefined ? "{}" : o.Data.GetRawText(),
        Items = o.Items.Select(i => new OrderItemResource
        {
            Id = i.Id, ProductId = i.ProductId, Quantity = i.Quantity, Price = i.Price
        }).ToList(),
        CreatedAt = o.AuditInfo?.CreatedAt,
        CreatedBy = o.AuditInfo?.CreatedBy
    };
}
