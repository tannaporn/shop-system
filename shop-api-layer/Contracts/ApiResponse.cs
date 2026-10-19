namespace shop_api_layer.Contracts;

public sealed class ApiResponse<T>
{
    public ApiResponse(T data)
    {
        Data = data;
        Success = true;
    }

    private ApiResponse(ApiError error)
    {
        Error = error;
    }

    public bool Success { get; }
    public T? Data { get; }
    public ApiError? Error { get; }

    public static ApiResponse<T> Failure(ApiError error) => new(error);
}
