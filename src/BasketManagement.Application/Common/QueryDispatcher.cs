using BasketManagement.Application.Common.Interfaces;
using MediatR;

namespace BasketManagement.Application.Common;

public class QueryDispatcher : IQueryDispatcher, IScopedDependency
{
    private readonly IMediator _mediator;
    public QueryDispatcher(IMediator mediator) => _mediator = mediator;
    public async Task<TResponse> Send<TResponse>(IQuery<TResponse> query,
        CancellationToken cancellationToken = default)
        => await _mediator.Send(query, cancellationToken);
}