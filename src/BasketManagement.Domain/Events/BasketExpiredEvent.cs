using BasketManagement.Domain.Interfaces;

namespace BasketManagement.Domain.Events;

public record BasketExpiredEvent(long BasketId, long UserId) : IDomainEvent;