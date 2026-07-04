namespace BasketManagement.Application.Common;

public interface ICommandDispatcher
{
    Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
}