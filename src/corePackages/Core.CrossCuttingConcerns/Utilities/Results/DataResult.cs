namespace Core.CrossCuttingConcerns.Utilities.Results;

public class DataResult<T>
{
    public T? Data { get; set; }
    public string? Message { get; set; }
    public bool Success { get; set; }

    public static DataResult<T> IsSuccess(T data, string message)
    {
        return new DataResult<T> { Data = data, Message = message, Success = true };
    }

    public static DataResult<T> IsSuccess(string message)
    {
        return new DataResult<T>
        {
            Data = default(T),
            Message = message,
            Success = true
        };

    }

    public static DataResult<T> IsError(T data, string message)
    {
        return new DataResult<T>
        {
            Data = data,
            Message = message,
            Success = false
        };
    }

    public static DataResult<T> IsError(string message)
    {
        return new DataResult<T>
        {
            Data = default(T),
            Message = message,
            Success = false
        };
    }
}
