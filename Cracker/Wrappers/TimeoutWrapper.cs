using System.Diagnostics;

namespace Cracker.Wrappers
{
    /// <summary>
    /// If the timeout time exceeds then this wrapper throws a cancellation token, telling the application to stop retrying.
    /// </summary>
    internal class TimeoutWrapper : WrapperBase
    {
        private readonly TimeSpan _period;

        public TimeoutWrapper(TimeSpan period)
        {
            _period = period;
        }

        public override async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> func, CancellationToken token)
        {
            Debug.WriteLine("Timeout Loaded.");

            // Create a new token source so we can cancel it in the timeout.
            using var timeoutTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);

            var ms = (int)_period.TotalMilliseconds;
            using var timer = new Timer(TimerCallback, timeoutTokenSource, ms, Timeout.Infinite);
            
            return await this.ExecuteInternalAsync(func, timeoutTokenSource.Token);
        }

        private void TimerCallback(object? obj)
        {
            var tokenSource = obj as CancellationTokenSource;

            Debug.WriteLine("Timeout triggered!");

            if (tokenSource != null)
            {
                tokenSource.Cancel(true);
            }
        }
    }
}
