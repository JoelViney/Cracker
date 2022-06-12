
namespace Cracker.Exceptions
{
    /// <summary>This exception is thrown when a timeout occurrs.</summary>
    public class TimeoutException: Exception
    {
        public TimeoutException() : base()
        {

        }

        public TimeoutException(string message): base(message)
        {

        }

        public TimeoutException(string message, Exception ex): base(message, ex)
        {

        }
    }
}
