using BytLabs.Application.DataAccess;
using BytLabs.Application.DomainEvents;
using BytLabs.MicroserviceTemplate.Application.Services;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate.Events;

namespace BytLabs.MicroserviceTemplate.Application.Events.OrderCreatedEvents
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
            var order = await orderRepository.GetByIdAsync(domainEvent.OrderId, cancellationToken);
            order.MarkAsEmailSent();

            await emailService.SendEmail("test@test.com", $"#{domainEvent.OrderId} order is placed.", "Welcome message...");

            await orderRepository.UpdateAsync(order, cancellationToken);
        }
    }
}
