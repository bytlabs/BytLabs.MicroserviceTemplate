namespace BytLabs.MicroserviceTemplate.Domain.Products.DataObjects
{
    // RECIPE: id-less input for a product variant. Used by Create/UpdateProduct so a client (the
    // console form) can send { sku, price } and let the domain generate the ProductVariant id.
    // Contrast with CreateVariant (carries an Id) used by the AddVariant sub-entity command.
    public record VariantData(string Sku, decimal Price);
}
