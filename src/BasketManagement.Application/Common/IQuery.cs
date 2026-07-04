using MediatR;

namespace BasketManagement.Application.Common;

public interface IQuery<out TResponse> : IRequest<TResponse> { }