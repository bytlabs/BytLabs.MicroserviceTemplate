using BytLabs.MicroserviceTemplate.Application.Products.Commands.CreateProduct;
using BytLabs.MicroserviceTemplate.Application.Products.Commands.UpdateProduct;
using BytLabs.MicroserviceTemplate.Application.Products.Commands.RemoveProduct;
using BytLabs.MicroserviceTemplate.Application.Products.Commands.AddVariant;
using BytLabs.MicroserviceTemplate.Application.Products.Commands.RemoveVariant;
using BytLabs.MicroserviceTemplate.Application.Products.Dtos;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using MediatR;
using BytLabs.Hotchocolate.ErrorTypes.Validation;
using BytLabs.Hotchocolate.ErrorTypes.Business;

namespace BytLabs.MicroserviceTemplate.Api.Graphql.Mutations
{
    public partial class Mutation
    {
        [Authorize]
        [Error(typeof(BusinessError))]
        [Error(typeof(ValidationError))]
        public async Task<ProductDto> CreateProduct(CreateProductCommand input, [Service] IMediator mediator, CancellationToken cancellationToken)
            => await mediator.Send(input, cancellationToken);

        [Authorize]
        [Error(typeof(BusinessError))]
        [Error(typeof(ValidationError))]
        public async Task<ProductDto> UpdateProduct(UpdateProductCommand input, [Service] IMediator mediator, CancellationToken cancellationToken)
            => await mediator.Send(input, cancellationToken);

        [Authorize]
        [Error(typeof(BusinessError))]
        [Error(typeof(ValidationError))]
        public async Task<RemoveProductResult> RemoveProduct(RemoveProductCommand input, [Service] IMediator mediator, CancellationToken cancellationToken)
            => await mediator.Send(input, cancellationToken);

        [Authorize]
        [Error(typeof(BusinessError))]
        [Error(typeof(ValidationError))]
        public async Task<ProductVariantDto> AddVariant(AddVariantCommand input, [Service] IMediator mediator, CancellationToken cancellationToken)
            => await mediator.Send(input, cancellationToken);

        [Authorize]
        [Error(typeof(BusinessError))]
        [Error(typeof(ValidationError))]
        public async Task<RemoveVariantResult> RemoveVariant(RemoveVariantCommand input, [Service] IMediator mediator, CancellationToken cancellationToken)
            => await mediator.Send(input, cancellationToken);
    }
}
