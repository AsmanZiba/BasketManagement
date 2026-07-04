using MediatR;
using Microsoft.Extensions.Logging;

namespace BasketManagement.Application.Basket.Events;

/// <summary>
/// مدیریت رویداد یکپارچه‌سازی: ثبت تاریخچه و عملیات جانبی
/// </summary>
public class BasketItemAddedIntegrationEventHandler : INotificationHandler<BasketItemAddedIntegrationEvent>
{
    private readonly ILogger<BasketItemAddedIntegrationEventHandler> _logger;

    public BasketItemAddedIntegrationEventHandler(ILogger<BasketItemAddedIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(BasketItemAddedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        //todo: send to message queue

        _logger.LogInformation(
            "رویداد یکپارچه‌سازی: کالا {ProductId} به سبد {BasketId} اضافه شد.",
            notification.ProductId,
            notification.BasketId
        );

        return Task.CompletedTask;
    }
}