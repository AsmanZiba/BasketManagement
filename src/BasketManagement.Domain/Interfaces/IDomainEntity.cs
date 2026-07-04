namespace BasketManagement.Domain.Interfaces;

public interface IDomainEntity
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}
