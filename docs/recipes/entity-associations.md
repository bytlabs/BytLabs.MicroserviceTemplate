# Entity associations & outer-field integration

**What it is.** How the dynamic Product/Order examples model **associations** (an entity referencing
another, and owned sub-collections) and **outer fields** (aggregate fields that live outside the
dynamic `Data` bag) — all driven by the `EntityDef` schema, with no per-field mapping code.

**When to use it.** When a dynamic entity needs a field that points at another entity (Order line item
→ Product), an editable sub-collection (Order `Items`, Product `Variants`), or a structured aggregate
field (Name, OrderDate) surfaced in the same form as its dynamic `Data`.

## The `data`-nesting convention (no field-source metadata)

The form schema mirrors the aggregate: **dynamic fields are nested under a `data` object; outer fields
are top-level siblings.** Because a field's *path* already says where it lives (`data.color` → dynamic
bag; `name`/`orderDate`/`items` → outer aggregate field), the RJSF `formData` — e.g.
`{ name, orderDate, items: [...], data: { color, … } }` — is **already** the GraphQL mutation input
shape. So the console mapping is pass-through (see [`lib/inputs.ts`](../../src/BytLabs.MicroserviceTemplate.Console/src/lib/inputs.ts)):

```ts
createInput = { [idField]: newId(), ...formData };  // + strip __typename, inject row ids where required
updateInput = { id, ...formData };
rowToFormData = pick(row, Object.keys(formSchema.properties)); // drops id/status/__typename
```

Columns and the detail view keep the `data.*` accessor convention, so form/table/view all agree.

## Association 1 — cross-entity reference (Order → Product)

An `Order.Items[].productId` points at a `Product`. In the schema, `items` is an outer array of objects
and `productId` uses the registry **`ReferenceWidget`**:

```jsonc
// order formSchema
"items": { "type": "array", "title": "Line items", "default": [], "items": {
  "type": "object", "required": ["productId", "quantity"],
  "properties": { "productId": {"type":"string"}, "quantity": {"type":"integer"}, "price": {"type":"number"} } } }
// order uiSchema
"items": { "items": { "productId": {
  "ui:widget": "ReferenceWidget", "ui:options": { "entity": "Product", "labelField": "name" } } } }
```

`ReferenceWidget` renders a `Select` whose options come from a surrounding **`EntityOptionsProvider`**;
the stored value is the referenced id. The console supplies `loadOptions(entity)` (queries the target
entity's list via Apollo → `{ value: id, label }`) and wraps the forms in the provider — see
[`EntityManager.tsx`](../../src/BytLabs.MicroserviceTemplate.Console/src/components/EntityManager.tsx).
With no provider the widget degrades to a plain text input, so the registry item stays standalone.

## Association 2 — owned sub-collection (Product → Variants)

`Product.Variants` is an array of `{ sku, price }` with no cross-entity link — an `outer` array field,
no widget config needed. Same shape for the structured half of Order `Items`.

## Backend: how the collections are set

- **Order items** already flow through `CreateOrderInput.items` (`OrderItemInput`, which requires an
  `id` per row — the console injects one via `rowIdFields: ['items']`; the domain regenerates it
  anyway). `UpdateOrder` now also accepts `items` and **replaces** the collection —
  [`Order.Update`](../../src/BytLabs.MicroserviceTemplate.Domain/Orders/Aggregates/Order.cs).
- **Product variants** use an id-less [`VariantData`](../../src/BytLabs.MicroserviceTemplate.Domain/Products/Inputs/VariantData.cs)
  input (`{ sku, price }`) so the form row stays clean; `Create`/`UpdateProduct` accept optional
  `variants` and the domain generates each `ProductVariant` id (replace semantics).
- The `AddVariant`/`RemoveVariant` commands remain for the sub-entity-command recipe; the form path uses
  replace for simplicity. See [Sub-entity commands](sub-entity-commands.md).

> **Gotcha — DateTime outer fields.** `CreateOrderInput.orderDate` is `DateTime`, so the `orderDate`
> field uses `format: "date-time"` (not `"date"`) — the `DatePickerWidget` then emits an ISO datetime
> the API accepts. A `format: "date"` field emits `"YYYY-MM-DD"`, which a `DateTime` input rejects.

**Sample code in this template.**
- [`order.schema.json`](../console-samples/order.schema.json), [`product.schema.json`](../console-samples/product.schema.json) — importable schemas (nested `data`, items+reference, variants)
- [`ReferenceWidget.tsx`](../../src/BytLabs.MicroserviceTemplate.Console/src/components/dynamic/rjsf/widgets/ReferenceWidget.tsx), [`EntityOptionsContext.tsx`](../../src/BytLabs.MicroserviceTemplate.Console/src/components/dynamic/rjsf/EntityOptionsContext.tsx) — the picker + its pluggable loader
- [`inputs.ts`](../../src/BytLabs.MicroserviceTemplate.Console/src/lib/inputs.ts), [`entities.ts`](../../src/BytLabs.MicroserviceTemplate.Console/src/lib/entities.ts) — pass-through builders + declarative registration

**Related recipes.** [EntityDef schema flow](entity-def-schema-flow.md), [UI registry integration](ui-registry-integration.md), [Dynamic data](dynamic-data.md), [Sub-entity commands](sub-entity-commands.md), [Bundled console app](console-app.md).
