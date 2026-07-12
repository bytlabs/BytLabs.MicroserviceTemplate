using BytLabs.Domain.Entities;
using BytLabs.MicroserviceTemplate.Domain.EntityDefs.DataObjects;
using BytLabs.MicroserviceTemplate.Domain.EntityDefs.Events;
using BytLabs.MicroserviceTemplate.Domain.Common.DynamicData;

namespace BytLabs.MicroserviceTemplate.Domain.EntityDefs
{
    // RECIPE HUB: definition aggregate describing how a dynamic entity's form and table render.
    // Stores only schemas (FormDataSchema/TableDataSchema); entity values live on the target
    // aggregate (Product/Order) as JsonElement Data. Soft-deletable; full create/update/remove.
    public sealed class EntityDef : AggregateRootBase<Guid>, ISoftDeletable
    {
        public string EntityType { get; private set; }
        public FormDataSchema Form { get; private set; }
        public TableDataSchema Table { get; private set; }
        public bool IsDeleted { get; private set; }

        public EntityDef(Guid id, string entityType, FormDataSchema form, TableDataSchema table) : base(id)
        {
            EntityType = entityType;
            Form = form;
            Table = table;
        }

        public static EntityDef Create(CreateEntityDef value)
        {
            var def = new EntityDef(value.Id, value.EntityType, value.Form, value.Table);
            def.AddDomainEvent(new EntityDefCreated(def.Id, value));
            return def;
        }

        // Update: replace form + table schemas wholesale (mirrors OrganizationSubEntityDef.Update).
        public void Update(UpdateEntityDef value)
        {
            Form = value.Form;
            Table = value.Table;
            AddDomainEvent(new EntityDefUpdated(Id, value));
        }

        // Soft delete: flag, don't remove. Queries exclude these via ExcludeSoftDeletedEntites().
        public void Remove()
        {
            IsDeleted = true;
            AddDomainEvent(new EntityDefRemoved(Id));
        }
    }
}
