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


        public override async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> func, CancellationToken token)
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
                    if (token.IsCancellationRequested)
                    {
                        throw new TaskCanceledException();
                    }

                    if (attempt > 1)
                    {
                        var comp = this.DelayExpression.Compile();
                        var delay = comp.Invoke(attempt);
                        await Task.Delay(delay, token);
                    }

                    // Check for a cancellation request
                    if (token.IsCancellationRequested)
                    {
                        throw new TaskCanceledException();
                    }

                    var result = await this.ExecuteInternalAsync<T>(func, token);

                    if (this.MatchingWhenResult<T>(result))
                    {
                        continue;
                    }

                    return result;
                }
                catch (TaskCanceledException ex)
                {
                    if (this.MatchingWhenExceptions(ex))
                    {
                        continue; // Retry When Exception ==...
                    }

                    exceptions.Add(ex);
                    break;
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
