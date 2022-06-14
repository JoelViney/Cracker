using System.Diagnostics;

namespace Cracker.Wrappers
{
    /// <summary>
    /// If the timeout time exceeds then this wrapper throws a cancellation token, telling the application to stop retrying.
    /// </summary>
    internal class TimeoutWrapper : WrapperBase
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly TimeSpan _period;

        public TimeoutWrapper(CancellationTokenSource cancellationTokenSource, TimeSpan period)
        {
            _cancellationTokenSource = cancellationTokenSource;
            _period = period;
        }

        public override async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> func)
        {
            Debug.WriteLine("Timeout Loaded.");

            var ms = (int)_period.TotalMilliseconds;
            using var timer = new Timer(TimerCallback, new AutoResetEvent(false), ms, 0);

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
