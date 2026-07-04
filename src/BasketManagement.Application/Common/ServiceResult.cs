namespace BasketManagement.Application.Common;

/// <summary>نتیجه عملیات بدون داده</summary>
public class ServiceResult
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }
    public int? ErrorCode { get; }

    protected ServiceResult(bool isSuccess, string? errorMessage = null, int? errorCode = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
    }

    public static ServiceResult Success() => new(true);
    public static ServiceResult Failure(string message, int code = 400) => new(false, message, code);
}

/// <summary>نتیجه عملیات با داده</summary>
public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; }

    private ServiceResult(
        bool isSuccess,
        T? data = default,
        string? errorMessage = null,
        int? errorCode = null) : base(isSuccess, errorMessage, errorCode)
    {
        Data = data;
    }

    public static ServiceResult<T> Success(T data) => new(true, data);
    public new static ServiceResult<T> Failure(string message, int code = 400)
        => new(false, default, message, code);
}