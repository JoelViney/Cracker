using System.Diagnostics;

namespace Cracker.Wrappers
{
    /// <summary>
    /// Limits the amount of calls for a set amount of time. e.g. 60 calls in 60 seconds.
    /// </summary>
    internal class ThrottleWrapper : WrapperBase
    {
        private readonly int _callLimit;
        private readonly TimeSpan _period;

        private readonly SemaphoreSlim _semaphore;
        private readonly Queue<long> _callTickStack;

        internal ThrottleWrapper(int callLimit, TimeSpan period)
        {
            _callLimit = callLimit;
            _period = period;

            _semaphore = new(1);
            _callTickStack = new(callLimit);
        }

        public override async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> func)
        {
            Debug.WriteLine("Throttle Loaded.");

            await _semaphore.WaitAsync();

            try
            {
                // Check if we need to sleep
                if (_callTickStack.Count >= _callLimit)
                {
                    long nowTicks = DateTime.UtcNow.Ticks;
                    var callAtLimitTicks = _callTickStack.Peek();

                    TimeSpan timeSpan = TimeSpan.FromTicks(nowTicks - callAtLimitTicks);

                    var callLimitMilliseconds = _period.TotalMilliseconds;
                    if (timeSpan.TotalMilliseconds < callLimitMilliseconds)
                    {
                        var millisecondsDelay = (int)(callLimitMilliseconds - Math.Round(timeSpan.TotalMilliseconds));

                        // Check for a cancellation request
                        if (this.CancellationToken.IsCancellationRequested)
                        {
                            throw new TimeoutException();
                        }

                        Debug.WriteLine($"Throttle going to sleep for {millisecondsDelay}ms.");
                        await Task.Delay(millisecondsDelay);
                    }
                }

                // Check for a cancellation request
                if (this.CancellationToken.IsCancellationRequested)
                {
                    throw new TimeoutException();
                }

                // Enqueue the execution and Dequeue old executions if needed
                var executionTimeTicks = DateTime.UtcNow.Ticks;
                _callTickStack.Enqueue(executionTimeTicks);

                if (_callTickStack.Count > _callLimit)
                {
                    _callTickStack.Dequeue(); // We only need to track as many calls as the call limit
                }

                return await this.ExecuteInternalAsync(func);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
