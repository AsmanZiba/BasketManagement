using MediatR;

namespace BasketManagement.Application.Common;

public interface ICommand<out TResponse> : IRequest<TResponse> { }
