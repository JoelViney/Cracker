using System.Diagnostics;

namespace Cracker.Stubs
{
    internal class SlowJobStub
    {
        public bool Completed { get; internal set; } = false;
        public int Attempts { get; internal set; }

        private readonly int _milliseconds;
        private readonly bool _throwCancelledException;

        /// <param name="milliseconds">How long the Execute will take to return a result.</param>
        public SlowJobStub(int milliseconds, bool throwCancelledException = true)
        {
            _milliseconds = milliseconds;
            _throwCancelledException = throwCancelledException;
            this.Attempts = 0;
        }

        public async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
        {
            Debug.WriteLine("ExecuteAsync...");

            this.Attempts++;

            if (_milliseconds > 0)
            {
                // TODO Mod the last result?
                var loops = Math.Ceiling((decimal)_milliseconds / 10);

                for (int i = 0; i < loops; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        this.Completed = false;
                        Debug.WriteLine("Method Cancelled!");

                        if (_throwCancelledException)
                        {
                            throw new TaskCanceledException();
                        }

                        return await Task.FromResult<bool>(false);
                    }

                    Debug.WriteLine("Sleep...");
                    await Task.Delay(10, cancellationToken);
                }
            }

            this.Completed = true;

            return await Task.FromResult<bool>(true);
        }
    }
}
