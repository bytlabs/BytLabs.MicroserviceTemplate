using BytLabs.Domain.Audit;
using BytLabs.Domain.DynamicData;

namespace BytLabs.MicroserviceTemplate.Application.EntityDefs.Dtos
{
    // RECIPE: DTO exposing the dynamic form/table definition + audit info. Used for AutoMapper
    // (command results) and MongoDB projection (As<EntityDefDto>) in the query.
    public record EntityDefDto(
        Guid Id,
        string EntityType,
        FormDataSchema Form,
        TableDataSchema Table,
        AuditInfo AuditInfo) : IAuditable;
}
