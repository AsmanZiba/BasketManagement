using MediatR;
using Microsoft.Extensions.Logging;
using BasketManagement.Domain.Events;

namespace BasketManagement.Application.Basket.Events;

/// <summary>
/// مدیریت رویداد دامنه و تبدیل به رویداد یکپارچه
/// </summary>
public class BasketItemAddedDomainEventHandler : INotificationHandler<BasketItemAddedDomainEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<BasketItemAddedDomainEventHandler> _logger;

    public BasketItemAddedDomainEventHandler(
        IMediator mediator,
        ILogger<BasketItemAddedDomainEventHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Handle(BasketItemAddedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        // 1. لاگ ثبت شود
        _logger.LogInformation(
            "رویداد دامنه: کالا با شناسه {ProductId} به سبد کاربر {UserId} اضافه شد.",
            domainEvent.ProductId,
            domainEvent.UserId
        );

        // 2. تبدیل به Integration Event
        var integrationEvent = new BasketItemAddedIntegrationEvent(
            domainEvent.BasketId,
            domainEvent.UserId,
            domainEvent.ProductId,
            domainEvent.Quantity
        );

        // 3. انتشار Integration Event (برای سیستم‌های خارجی)
        await _mediator.Publish(integrationEvent, cancellationToken);
    }
}