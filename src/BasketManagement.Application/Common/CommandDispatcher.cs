using BasketManagement.Application.Common.Interfaces;
using MediatR;

namespace BasketManagement.Application.Common;

public class CommandDispatcher : ICommandDispatcher, IScopedDependency
{
    private readonly IMediator _mediator;
    public CommandDispatcher(IMediator mediator) => _mediator = mediator;
    public async Task<TResponse> Send<TResponse>(ICommand<TResponse> command,
        CancellationToken cancellationToken = default)
        => await _mediator.Send(command, cancellationToken);
}
