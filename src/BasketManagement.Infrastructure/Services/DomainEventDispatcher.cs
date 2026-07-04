using MediatR;
using BasketManagement.Domain.Interfaces;

namespace BasketManagement.Infrastructure.Services;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;

    public DomainEventDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task DispatchAsync(
        IEnumerable<IDomainEvent> events,
        CancellationToken cancellationToken = default)
    {
        foreach (var @event in events)
        {
            // تبدیل IDomainEvent به INotification با استفاده از Reflection
            // یا استفاده از یک Wrapper
            await _mediator.Publish(@event, cancellationToken);
        }
    }
}