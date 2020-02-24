namespace ManualTimeLogger.Domain
{
    public class ParseResult<T>
    {
        public bool IsSuccess { get; }

        public T Value { get; }

        public ParseResult(bool isSuccess, T value)
        {
            IsSuccess = isSuccess;
            Value = value;
        }
    }
}