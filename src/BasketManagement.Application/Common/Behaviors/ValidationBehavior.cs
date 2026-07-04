using FluentValidation;
using MediatR;
using BasketManagement.Application.Common;

namespace BasketManagement.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            var errorMessage = string.Join("; ", failures.Select(f => f.ErrorMessage));
            return CreateFailureResponse<TResponse>(errorMessage);
        }

        return await next();
    }

    private static TResponse CreateFailureResponse<T>(string errorMessage)
    {
        var resultType = typeof(ServiceResult);
        if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(ServiceResult<>))
        {
            var genericType = typeof(T).GetGenericArguments()[0];
            var failureMethod = typeof(ServiceResult<>)
                .MakeGenericType(genericType)
                .GetMethod(nameof(ServiceResult<object>.Failure), new[] { typeof(string), typeof(int) });
            return (TResponse)failureMethod!.Invoke(null, new object[] { errorMessage, 400 })!;
        }
        else if (typeof(T) == typeof(ServiceResult))
        {
            return (TResponse)(object)ServiceResult.Failure(errorMessage, 400);
        }
        throw new InvalidOperationException("نوع پاسخ پشتیبانی نمی‌شود.");
    }
}