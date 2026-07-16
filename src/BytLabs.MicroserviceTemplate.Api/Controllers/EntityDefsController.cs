using System.Text.Json;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Commands.CreateEntityDef;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Commands.RemoveEntityDef;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Commands.UpdateEntityDef;
using BytLabs.MicroserviceTemplate.Domain.Common.DynamicData;
using BytLabs.MicroserviceTemplate.Domain.EntityDefs.Aggregates;
using BytLabs.MicroserviceTemplate.Api.OData.Resources;
using BytLabs.MicroserviceTemplate.Api.Querying;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace BytLabs.MicroserviceTemplate.Api.Controllers;

public class EntityDefsController : ODataController
{
    private readonly IMediator _mediator;
    private readonly IQueryable<EntityDef> _entityDefs;

    // Form/Table schemas are transported as raw JSON strings and (de)serialized with the same options
    // used by the persistence layer so the value-object shape round-trips faithfully.
    private static readonly JsonSerializerOptions SchemaJson = new();

    public EntityDefsController(IMediator mediator, IQueryable<EntityDef> entityDefs)
    {
        _mediator = mediator;
        _entityDefs = entityDefs.ExcludeSoftDeletedEntities();
    }

    [EnableQuery]
    public ActionResult<IEnumerable<EntityDefResource>> Get()
        => Ok(_entityDefs.ToList().Select(Map));

    [EnableQuery]
    public ActionResult<EntityDefResource> Get([FromRoute] Guid key)
    {
        var def = _entityDefs.Where(d => d.Id == key).ToList().FirstOrDefault();
        return def is null ? NotFound() : Ok(Map(def));
    }

    public async Task<IActionResult> Post([FromBody] EntityDefResource resource, CancellationToken ct)
    {
        var command = new CreateEntityDefCommand(
            resource.Id == Guid.Empty ? Guid.NewGuid() : resource.Id,
            resource.EntityType,
            ParseForm(resource.Form),
            ParseTable(resource.Table));

        var dto = await _mediator.Send(command, ct);
        var created = _entityDefs.Where(d => d.Id == dto.Id).ToList().First();
        return Created(Map(created));
    }

    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] EntityDefResource resource, CancellationToken ct)
    {
        var command = new UpdateEntityDefCommand(key, ParseForm(resource.Form), ParseTable(resource.Table));
        var dto = await _mediator.Send(command, ct);
        var updated = _entityDefs.Where(d => d.Id == dto.Id).ToList().First();
        return Updated(Map(updated));
    }

    public async Task<IActionResult> Delete([FromRoute] Guid key, CancellationToken ct)
    {
        await _mediator.Send(new RemoveEntityDefCommand(key), ct);
        return NoContent();
    }

    private static FormDataSchema ParseForm(string? json)
        => string.IsNullOrWhiteSpace(json) ? FormDataSchema.Empty()
            : JsonSerializer.Deserialize<FormDataSchema>(json, SchemaJson)!;

    private static TableDataSchema ParseTable(string? json)
        => JsonSerializer.Deserialize<TableDataSchema>(
            string.IsNullOrWhiteSpace(json)
                ? "{\"SampleData\":{\"Type\":\"\",\"Data\":\"\"},\"Columns\":{\"Type\":\"\",\"Data\":\"\"},\"Filter\":{\"Type\":\"\",\"Data\":\"\"},\"Details\":{\"Type\":\"\",\"Data\":\"\"}}"
                : json, SchemaJson)!;

    private static EntityDefResource Map(EntityDef d) => new()
    {
        Id = d.Id,
        EntityType = d.EntityType,
        Form = JsonSerializer.Serialize(d.Form, SchemaJson),
        Table = JsonSerializer.Serialize(d.Table, SchemaJson),
        CreatedAt = d.AuditInfo?.CreatedAt,
        CreatedBy = d.AuditInfo?.CreatedBy
    };
}
