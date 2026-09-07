namespace LabConsumableExpiryTracker.Commons.Result;

public class ServiceResult<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string[] Errors { get; init; } = [];

    public static ServiceResult<T> Ok(T data)
    {
        return new ServiceResult<T>
        {
            Success = true,
            Data = data
        };
    }

    public static ServiceResult<T> Fail(params string[] errors)
    {
        return new ServiceResult<T>
        {
            Success = false,
            Errors = errors
        };
    }
}
