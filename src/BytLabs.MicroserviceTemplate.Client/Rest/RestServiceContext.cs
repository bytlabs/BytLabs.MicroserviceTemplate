using System.Xml;
using Microsoft.OData.Client;
using Microsoft.OData.Edm.Csdl;

namespace BytLabs.MicroserviceTemplate.Client.Rest;

/// <summary>
/// Typed OData client for the REST surface — the REST analogue of the StrawberryShake GraphQL client.
/// The service model is loaded once from the API's <c>$metadata</c> using the supplied
/// <see cref="HttpClient"/>, so the same client works against an in-memory test server or a real host.
/// </summary>
public class RestServiceContext : DataServiceContext
{
    public RestServiceContext(Uri serviceRoot, HttpClient httpClient)
        : base(serviceRoot, ODataProtocolVersion.V4)
    {
        // Route every OData request through the supplied HttpClient (test server or real host).
        Configurations.RequestPipeline.OnMessageCreating =
            args => new HttpClientRequestMessage(args, httpClient);

        Format.LoadServiceModel = () =>
        {
            using var stream = httpClient
                .GetStreamAsync(new Uri(serviceRoot, "$metadata"))
                .GetAwaiter().GetResult();
            using var reader = XmlReader.Create(stream);
            return CsdlReader.Parse(reader);
        };
        Format.UseJson();
    }

    public DataServiceQuery<OrderResource> Orders => CreateQuery<OrderResource>("Orders");
    public DataServiceQuery<ProductResource> Products => CreateQuery<ProductResource>("Products");
    public DataServiceQuery<EntityDefResource> EntityDefs => CreateQuery<EntityDefResource>("EntityDefs");
}
