using System.Linq.Expressions;
using Cracker.Helpers;
using Cracker.Rules;

namespace Cracker.Wrappers
{
    internal abstract class WrapperBase
    {
        internal WrapperBase? InnerWrapper { get; set; }

        public async Task ExecuteAsync(Func<CancellationToken, Task> func, CancellationToken token)
        {
            var teeFunc = TaskHelper.WrapTaskInGenericObject(func);

            await ExecuteAsync<object?>(teeFunc, token);
        }


        public abstract Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> func, CancellationToken token);


        protected async Task ExecuteInternalAsync(Func<CancellationToken, Task> func, CancellationToken token)
        {
            if (this.InnerWrapper != null)
            {
                await this.InnerWrapper.ExecuteAsync(func, token);
            }
            else
            {
                await func(token);
            }
        }

        protected async Task<T> ExecuteInternalAsync<T>(Func<CancellationToken, Task<T>> func, CancellationToken token)
        {
            if (this.InnerWrapper != null)
            {
                return await this.InnerWrapper.ExecuteAsync<T>(func, token);
            }
            else
            {
                return await func(token);
            }
        }
    }
}
