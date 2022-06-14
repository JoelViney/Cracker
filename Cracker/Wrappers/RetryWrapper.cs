using System.Diagnostics;

namespace Cracker.Wrappers
{
    /// <summary>
    /// Provides Retry on error functionality with minimal impact on the calling code.
    /// </summary>
    internal class RetryWrapper : RetryWrapperExBase
    {
        private readonly int _attempts;

        internal RetryWrapper(int attempts) : base()
        {
            _attempts = attempts;
        }


        public override async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> func)
        {
            Debug.WriteLine("Retry Loaded.");

            var exceptions = new List<Exception>();

            var totalAttempts = _attempts + 1;
            for (int attempt = 1; attempt <= totalAttempts; attempt++)
            {
                Debug.WriteLine($"Attempt: {attempt}");
                try
                {
                    // Check for a cancellation request
                    if (this.CancellationToken.IsCancellationRequested)
                    {
                        throw new TimeoutException();
                    }

                    if (attempt > 1)
                    {
                        var comp = this.DelayExpression.Compile();
                        var delay = comp.Invoke(attempt);
                        await Task.Delay(delay);
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
