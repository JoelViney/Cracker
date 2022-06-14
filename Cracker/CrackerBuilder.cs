using System.Linq.Expressions;
using Cracker.Helpers;
using Cracker.Wrappers;

namespace Cracker
{
    public class CrackerBuilder
    {
        internal CancellationTokenSource CancellationTokenSource { get; }
        internal CancellationToken CancellationToken { get; }

        internal WrapperBase? Command { get; set; }


        #region Constructors...

        public CrackerBuilder()
        {
            this.CancellationTokenSource = new();
            this.CancellationToken = CancellationTokenSource.Token;
            this.Command = null;
        }

        public CrackerBuilder(CrackerBuilder builder)
        {
            this.CancellationTokenSource = builder.CancellationTokenSource;
            this.CancellationToken = builder.CancellationToken;
            this.Command = builder.Command;
        }

        #endregion

        private void AddCommand(WrapperBase command)
        {
            command.InnerWrapper = Command;
            command.CancellationToken = CancellationToken;
            this.Command = command;
        }

        public CrackerBuilder Timeout(int timeoutMilliseconds)
        {
            var command = new TimeoutWrapper(CancellationTokenSource, TimeSpan.FromMilliseconds(timeoutMilliseconds));
            AddCommand(command);
            return this;
        }
        public CrackerBuilder Timeout(TimeSpan period)
        {
            var command = new TimeoutWrapper(CancellationTokenSource, period);
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
            if (this.Command == null)
            {
                await func(this.CancellationToken);
            }
            else
            {
                await this.Command.ExecuteAsync(func);
            }
        }

        public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> func)
        {
            if (this.Command == null)
            {
                return await func(this.CancellationToken);
            }
            else
            {
                return await this.Command.ExecuteAsync<T>(func);
            }
        }
    }
}