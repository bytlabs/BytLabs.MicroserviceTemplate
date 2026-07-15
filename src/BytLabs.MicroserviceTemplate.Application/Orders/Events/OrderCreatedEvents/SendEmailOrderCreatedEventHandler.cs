using BytLabs.Application.DataAccess;
using BytLabs.Application.DomainEvents;
using BytLabs.MicroserviceTemplate.Application.Common.Services;
using BytLabs.MicroserviceTemplate.Domain.Orders.Aggregates;
using BytLabs.MicroserviceTemplate.Domain.Orders.Entities;
using BytLabs.MicroserviceTemplate.Domain.Orders.ValueObjects;
using BytLabs.MicroserviceTemplate.Domain.Orders.Events;

namespace BytLabs.MicroserviceTemplate.Application.Orders.Events.OrderCreatedEvents
{
    public class SendEmailOrderCreatedEventHandler : DomainEventHandler<OrderCreatedEvent>
    {
        private readonly IRepository<Order, Guid> orderRepository;
        private readonly IEmailService emailService;

        public SendEmailOrderCreatedEventHandler(IRepository<Order, Guid> orderRepository,
            IEmailService emailService)
        {
            this.orderRepository = orderRepository;
            this.emailService = emailService;
        }

        protected async override Task HandleDomainEvent(OrderCreatedEvent domainEvent, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdAsync(domainEvent.Id, cancellationToken);
            order.MarkAsEmailSent();

            await emailService.SendEmail("test@test.com", $"#{domainEvent.Id} order is placed.", "Welcome message...");

            await orderRepository.UpdateAsync(order, cancellationToken);
        }
    }
}
