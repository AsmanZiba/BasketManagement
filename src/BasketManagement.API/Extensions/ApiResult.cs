namespace BasketManagement.API.Extensions;

public class ApiResult
{
    public bool IsSuccess { get; set; }
    public object? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public int? ErrorCode { get; set; }
}
