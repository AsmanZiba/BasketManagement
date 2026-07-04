namespace BasketManagement.Application.Common;

public interface IQueryDispatcher
{
    Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
}
