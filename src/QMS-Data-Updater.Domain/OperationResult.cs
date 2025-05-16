namespace QMS_Data_Updater.Domain;

public class OperationResult : IOperationResult
{
    public bool IsSuccess { get; private set; }
    public string Message { get; private set; }
    public Exception? Exception { get; private set; }
    public int StatusCode { get; private set; }
    public object? Data { get; private set; }

    protected OperationResult(bool isSuccess, string message, int statusCode, Exception? exception = null,
        object? data = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        StatusCode = statusCode;
        Exception = exception;
        Data = data;
    }

    public static OperationResult Success(string message = "Success", int statusCode = 200, object? data = null)
        => new OperationResult(true, message, statusCode, null, data);

    public static OperationResult Failure(string message, int statusCode = 500, Exception? exception = null,
        object? data = null)
        => new OperationResult(false, message, statusCode, exception, data);
}

public class OperationResult<T> : OperationResult, IOperationResult<T>
{
    public T Result { get; private set; }

    private OperationResult(bool isSuccess, string message, int statusCode, T result, Exception? exception = null,
        object? data = null)
        : base(isSuccess, message, statusCode, exception, data)
    {
        Result = result;
    }

    public static OperationResult<T> Success(T result, string message = "Success", int statusCode = 200,
        object? data = null)
        => new OperationResult<T>(true, message, statusCode, result, null, data);

    public static OperationResult<T> Failure(string message, int statusCode = 500, Exception? exception = null,
        object? data = null)
        => new OperationResult<T>(false, message, statusCode, default!, exception, data);
}