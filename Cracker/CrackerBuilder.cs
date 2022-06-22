using System.Linq.Expressions;
using Cracker.Helpers;
using Cracker.Wrappers;

namespace Cracker
{
    public class CrackerBuilder
    {
        internal WrapperBase? Command { get; set; }


        #region Constructors...

        public CrackerBuilder()
        {
            this.Command = null;
        }

        public CrackerBuilder(CrackerBuilder builder)
        {
            this.Command = builder.Command;
        }

        #endregion

        private void AddCommand(WrapperBase command)
        {
            command.InnerWrapper = Command;
            this.Command = command;
        }

        public CrackerBuilder Timeout(int timeoutMilliseconds)
        {
            var command = new TimeoutWrapper(TimeSpan.FromMilliseconds(timeoutMilliseconds));
            AddCommand(command);
            return this;
        }
        public CrackerBuilder Timeout(TimeSpan period)
        {
            var command = new TimeoutWrapper(period);
            AddCommand(command);
            return this;
        }

        public CrackerBuilderExRetry Retry(int retryAttempts)
        {
            var command = new RetryWrapper(retryAttempts);
            AddCommand(command);
            return new CrackerBuilderExRetry(this);
        }

        public CrackerBuilder Throttle(int callLimit, TimeSpan period)
        {
            var command = new ThrottleWrapper(callLimit, period);
            AddCommand(command);
            return this;
        }
        public CrackerBuilder Throttle(int callLimit, int timePeriodMilliseconds)
        {
            var command = new ThrottleWrapper(callLimit, TimeSpan.FromMilliseconds(timePeriodMilliseconds));
            AddCommand(command);
            return this;
        }


        public async Task ExecuteAsync(Func<CancellationToken, Task> func)
        {
            using var source = new CancellationTokenSource();

            if (this.Command == null)
            {
                await func(source.Token);
            }
            else
            {
                await this.Command.ExecuteAsync(func, source.Token);
            }
        }

        public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> func)
        {
            using var source = new CancellationTokenSource();

            if (this.Command == null)
            {
                return await func(source.Token);
            }
            else
            {
                return await this.Command.ExecuteAsync<T>(func, source.Token);
            }
        }
    }
}