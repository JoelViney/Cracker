using System.Linq.Expressions;
using Cracker.Helpers;
using Cracker.Rules;

namespace Cracker.Wrappers
{
    internal abstract class WrapperBase
    {
        public CancellationToken CancellationToken { get; internal set; }

        internal WrapperBase? InnerWrapper { get; set; }

        public async Task ExecuteAsync(Func<CancellationToken, Task> func)
        {
            var teeFunc = TaskHelper.WrapTaskInGenericObject(func);

            await ExecuteAsync<object?>(teeFunc);
        }


        public abstract Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> func);


        protected async Task ExecuteInternalAsync(Func<CancellationToken, Task> func)
        {
            if (this.InnerWrapper != null)
            {
                await this.InnerWrapper.ExecuteAsync(func);
            }
            else
            {
                await func(this.CancellationToken);
            }
        }

        protected async Task<T> ExecuteInternalAsync<T>(Func<CancellationToken, Task<T>> func)
        {
            if (this.InnerWrapper != null)
            {
                return await this.InnerWrapper.ExecuteAsync<T>(func);
            }
            else
            {
                return await func(this.CancellationToken);
            }
        }
    }
}
