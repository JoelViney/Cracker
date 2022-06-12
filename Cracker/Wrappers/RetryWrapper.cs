using System.Diagnostics;

namespace Cracker.Wrappers
{
    /// <summary>
    /// Provides Retry on error functionality with minimal impact on the calling code.
    /// </summary>
    internal class RetryWrapper : WrapperExBase
    {
        public const int RetryIntervalMilliseconds = 1000;

        private readonly int _retryAttempts;
        private readonly int _retryIntervalMilliseconds;

        internal RetryWrapper(int retryAttempts, int retryIntervalMilliseconds = RetryIntervalMilliseconds)
        {
            _retryAttempts = retryAttempts;
            _retryIntervalMilliseconds = retryIntervalMilliseconds;
        }

        public override async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> func)
        {
            Debug.WriteLine("Retry Loaded.");

            var exceptions = new List<Exception>();

            // Attempt = 1, retry
            var totalAttempts = _retryAttempts + 1;
            for (int attempts = 1; attempts <= totalAttempts; attempts++)
            {
                Debug.WriteLine($"Retry Attempt: {attempts}");
                try
                {
                    // Check for a cancellation request
                    if (this.CancellationToken.IsCancellationRequested)
                    {
                        throw new TimeoutException();
                    }

                    if (attempts > 1)
                    {
                        var multiplier = (int)Math.Pow(2, attempts);
                        await Task.Delay(_retryIntervalMilliseconds * multiplier);
                    }

                    // Check for a cancellation request
                    if (this.CancellationToken.IsCancellationRequested)
                    {
                        throw new TimeoutException();
                    }

                    var result = await this.ExecuteInternalAsync<T>(func);

                    if (this.MatchingWhenResult<T>(result))
                    {
                        continue;
                    }

                    return result;
                }
                catch (TimeoutException ex)
                {
                    if (this.MatchingWhenExceptions(ex))
                    {
                        continue; // Retry When Exception ==...
                    }

                    exceptions.Add(ex);
                }
                catch (Exception ex)
                {
                    if (this.MatchingUnlessExceptions(ex))
                    {
                        exceptions.Add(ex);
                        break;
                    }

                    if (this.MatchingWhenExceptions(ex))
                    {
                        continue;
                    }

                    exceptions.Add(ex);
                }
            }

            // Build up Exceptions
            bool different = false;
            foreach (var ex in exceptions.Skip(1))
            {
                if (exceptions[0].ToString() != ex.ToString())
                {
                    different = true;
                    break;
                }
            }

            if (!different)
            {
                throw exceptions[0];
            }

            throw new AggregateException(exceptions);
        }
    }
}
