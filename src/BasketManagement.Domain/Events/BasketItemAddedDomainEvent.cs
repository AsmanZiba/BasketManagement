
using MediatR;
using BasketManagement.Domain.Interfaces;

namespace BasketManagement.Domain.Events;

/// <summary>
/// رویداد دامنه: افزودن کالا به سبد
/// </summary>
/// <param name="BasketId"></param>
/// <param name="UserId"></param>
/// <param name="ProductId"></param>
/// <param name="Quantity"></param>
public record BasketItemAddedDomainEvent(long BasketId, long UserId, long ProductId, int Quantity) :
    IDomainEvent, INotification;