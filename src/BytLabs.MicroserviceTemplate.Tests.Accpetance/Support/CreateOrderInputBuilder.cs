using BytLabs.MicroserviceTemplate.Client;
using BuilderGenerator;

namespace BytLabs.MicroserviceTemplate.Tests.Accpetance.Support
{
    [BuilderFor(typeof(CreateOrderInput))]
    public partial class CreateOrderInputBuilder
    {
    }

    [BuilderFor(typeof(OrderItemInput))]
    public partial class OrderItemInputBuilder
    {
    }    
}
