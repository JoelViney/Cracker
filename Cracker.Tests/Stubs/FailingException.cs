
namespace Cracker.Stubs
{
    /// <summary>
    /// This is an exception returned by the FailingJobStub.
    /// </summary>
    public class FailingException : Exception
    {
        public int Attempts { get; private set; }

        public FailingException(int attempts, string message) : base(message)
        {
            this.Attempts = attempts;
        }
    }
}
