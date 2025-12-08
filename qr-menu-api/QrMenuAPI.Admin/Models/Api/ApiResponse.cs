namespace QrMenuAPI.Admin.Models.Api;

public class ApiResponse
{
    public bool Success { get; set; }
    public object? Data { get; set; }
    public string? ErrorCode { get; set; }

    public ApiResponse()
    {
        Success = true;
        Data = default;
        ErrorCode = null;
    }

    public ApiResponse(object? data)
    {
        Success = true;
        Data = data;
        ErrorCode = null;
    }

    public ApiResponse(string errorCode, object? data = null)
    {
        Success = false;
        Data = data;
        ErrorCode = errorCode;
    }
}
