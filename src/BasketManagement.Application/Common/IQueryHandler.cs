using MediatR;

namespace BasketManagement.Application.Common;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse> { }