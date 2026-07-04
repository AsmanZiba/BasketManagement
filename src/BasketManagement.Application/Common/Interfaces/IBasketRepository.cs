using BasketManagement.Domain.Interfaces;

namespace BasketManagement.Application.Interfaces;

public interface IBasketRepository : IRepository<Domain.Entities.Basket>
{
    Task<Domain.Entities.Basket?> GetActiveBasketByUserIdAsync(long userId, CancellationToken cancellationToken = default);
    Task<List<Domain.Entities.Basket>> GetExpiredBasketsAsync(CancellationToken cancellationToken = default);
}