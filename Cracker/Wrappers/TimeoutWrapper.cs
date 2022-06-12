using System.Diagnostics;

namespace Cracker.Wrappers
{
    /// <summary>
    /// If the timeout time exceeds then this wrapper throws a cancellation token, telling the application to stop retrying.
    /// </summary>
    internal class TimeoutWrapper : WrapperBase
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly int _timeoutMilliseconds;

        public TimeoutWrapper(CancellationTokenSource cancellationTokenSource, int timeoutMilliseconds)
        {
            _cancellationTokenSource = cancellationTokenSource;
            _timeoutMilliseconds = timeoutMilliseconds;
        }

        public override async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> func)
        {
            Debug.WriteLine("Timeout Loaded.");

            using var timer = new Timer(TimerCallback, new AutoResetEvent(false), _timeoutMilliseconds, 0);

            return await this.ExecuteInternalAsync(func);
        }

        private void TimerCallback(object? state)
        {
            if (state is AutoResetEvent)
            {
                Debug.WriteLine("TimerCallback AutoResetEvent.");
            }

            Debug.WriteLine("Timeout triggered!");

            _cancellationTokenSource.Cancel(true);
        }
    }
}
