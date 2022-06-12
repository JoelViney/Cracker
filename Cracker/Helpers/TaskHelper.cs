
namespace Cracker.Helpers
{
    internal static class TaskHelper
    {
        /// <summary>
        /// Wraps a Func<Task> into a generic Task so we can call a generics based method.
        /// </summary>
        public static Func<CancellationToken, Task<object?>> WrapTaskInGenericObject(Func<CancellationToken, Task> func)
        {
            return async cancellationToken =>
            {
                await func(cancellationToken);
                return null;
            };
        }
    }
}
