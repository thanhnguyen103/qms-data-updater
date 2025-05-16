namespace QMS_Data_Updater.Domain
{
    public interface IOperationResult
    {
        bool IsSuccess { get; }
        string Message { get; }
        Exception? Exception { get; }
        int StatusCode { get; }
        object? Data { get; }
    }

    public interface IOperationResult<T> : IOperationResult
    {
        T Result { get; }
    }
}