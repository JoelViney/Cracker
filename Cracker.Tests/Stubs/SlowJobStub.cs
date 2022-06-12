using System.Diagnostics;

namespace Cracker.Stubs
{
    internal class SlowJobStub
    {
        public bool Completed { get; internal set; } = false;
        public int Attempts { get; internal set; }

        private readonly int _milliseconds;

        /// <param name="milliseconds">How long the Execute will take to return a result.</param>
        public SlowJobStub(int milliseconds)
        {
            _milliseconds = milliseconds;
            this.Attempts = 0;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
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
                        return;
                    }

                    Debug.WriteLine("Sleep...");
                    await Task.Delay(10);
                }
            }

            this.Completed = true;
            await Task.FromResult<object?>(null);
        }
    }
}
