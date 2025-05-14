namespace BackEnd.Exceptions
{
    public class AppException : Exception
    {
        public int ErrorCode { get; }

        public AppException(string message, int errorCode = 400) : base(message)
        {
            ErrorCode = errorCode;
        }
    }

}
