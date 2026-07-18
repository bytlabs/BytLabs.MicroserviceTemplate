using BytLabs.Domain.Audit;
using BytLabs.Domain.DynamicData;

namespace BytLabs.MicroserviceTemplate.Application.EntityDefs.Dtos
{
    // RECIPE: DTO exposing the dynamic form/table definition + audit info. A plain get/set POCO (with a
    // parameterless ctor). Used for AutoMapper (command results) and MongoDB projection (As<EntityDefDto>).
    public class EntityDefDto : IAuditable
    {
        public Guid Id { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public FormDataSchema Form { get; set; } = default!;
        public TableDataSchema Table { get; set; } = default!;
        public AuditInfo AuditInfo { get; set; } = default!;
    }
}
