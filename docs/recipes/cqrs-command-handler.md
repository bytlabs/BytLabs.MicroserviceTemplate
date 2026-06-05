# Command + handler

**What it is.** The write side of CQRS: a `record` implementing `ICommand<TResult>` plus an
`ICommandHandler<TCommand, TResult>` that loads, mutates, and persists an aggregate through
`IRepository<TAgg, TId>`.

**When to use it.** For every state-changing operation. Queries use GraphQL resolvers instead.

**How it works.** The command carries input; the handler is resolved by MediatR (registered via
`AddCQS`), performs the work against the repository, and returns a DTO/result.

```csharp
public record CreateProductCommand(Guid Id, string Name, JsonElement Data) : ICommand<ProductDto>;

public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = Product.Create(new(request.Id, request.Name, request.Data, FormDataSchema.Empty()));
        var result = await productRepository.InsertAsync(product, ct);
        return mapper.Map<ProductDto>(result);
    }
}
```

**Sample code in this template.**
- [`Commands/CreateProduct/`](../../src/BytLabs.MicroserviceTemplate.Application/Commands/CreateProduct) — command + handler
- [`Commands/CreateOrder/`](../../src/BytLabs.MicroserviceTemplate.Application/Commands/CreateOrder) — minimal example

**Reference (BytLabs.BackendPackages).** `BytLabs.Application.CQS.Commands.ICommand`/`ICommandHandler`, `BytLabs.Application.DataAccess.IRepository<,>`, `AddCQS`.

**Related recipes.** [DTOs](dtos.md), [AutoMapper profiles](automapper-profiles.md), [Data objects](data-objects.md).
