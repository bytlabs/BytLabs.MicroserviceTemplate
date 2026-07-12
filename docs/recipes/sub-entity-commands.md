# Sub-entity add/remove commands

**What it is.** Commands that mutate a child collection on an existing aggregate root
(`AddVariant`/`RemoveVariant`) rather than creating or replacing the whole root.

**When to use it.** To manage owned sub-entities ([sub-entities](sub-entity.md)) after the root
exists — adding an order line, a product variant, an organization member.

**How it works.** The handler loads the root, calls the specialized method that mutates the owned
collection and raises an event, then saves the root via `UpdateAsync`.

```csharp
public async Task<ProductVariantDto> Handle(AddVariantCommand request, CancellationToken ct)
{
    var variant = ProductVariant.Create(new CreateVariant(request.VariantId, request.Sku, request.Price));
    var product = await productRepository.GetByIdAsync(request.ProductId, ct);
    product.AddVariant(new(variant));
    await productRepository.UpdateAsync(product, ct);
    return mapper.Map<ProductVariantDto>(variant);
}
```

**Sample code in this template.**
- [`Products/Commands/AddVariant/`](../../src/BytLabs.MicroserviceTemplate.Application/Products/Commands/AddVariant)
- [`Products/Commands/RemoveVariant/`](../../src/BytLabs.MicroserviceTemplate.Application/Products/Commands/RemoveVariant)
- [`Product.cs`](../../src/BytLabs.MicroserviceTemplate.Domain/Products/Product.cs) — `AddVariant`/`RemoveVariant`

**Reference (BytLabs.BackendPackages).** `BytLabs.Application.DataAccess.IRepository<,>`.

**Related recipes.** [Sub-entity](sub-entity.md), [Command + handler](cqrs-command-handler.md).
