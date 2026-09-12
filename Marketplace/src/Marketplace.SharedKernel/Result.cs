namespace Marketplace.SharedKernel
{
    public enum ErrorType
    {
        Validation,
        NotFound,
        Conflict,
        Unauthorized
    }

    public class Result
    {
        public bool IsSuccess { get; }
        public string? Error { get; }
        public ErrorType? ErrorType { get; }

        protected Result(bool isSuccess, string? error, ErrorType? errorType)
        {
            if (isSuccess && error is not null)
                throw new InvalidOperationException("A successful result cannot have an error.");
            if (!isSuccess && error is null)
                throw new InvalidOperationException("A failed result must have an error.");
            
            IsSuccess = isSuccess;
            Error = error;
            ErrorType = errorType;
        }

        public static Result Success() => new(true, null, null);
        public static Result Failure(string error, ErrorType errorType = SharedKernel.ErrorType.Validation) 
            => new(false, error, errorType);

        
    }

    public sealed class Result<T> : Result
    {
        public T? Value { get; }

        private Result(bool isSuccess, T? value, string? error, ErrorType? errorType) : base(isSuccess, error, errorType)
        {
            Value = value;
        }

        public static Result<T> Success(T value) => new(true, value, null, null);
        public static new Result<T> Failure(string error, ErrorType errorType = SharedKernel.ErrorType.Validation) => new(false, default, error, errorType);
    }
}