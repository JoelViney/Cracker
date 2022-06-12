using System.Diagnostics;

namespace Cracker.Stubs
{
    internal class FailingJobStub
    {
        private readonly int _fails;


        public bool Completed { get; internal set; }
        public int Attempts { get; internal set; }


        /// <param name="fails">How many times this task will fail before succeeding.</param>
        public FailingJobStub(int fails)
        {
            _fails = fails;

            this.Completed = false;
            this.Attempts = 0;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Debug.WriteLine("ExecuteAsync...");

            this.Attempts++;

            if (_fails >= this.Attempts)
            {
                throw new FailingException(this.Attempts, $"This is expected to fail {_fails} times and has failed {this.Attempts}.");
            }

            this.Completed = true;
            await Task.FromResult<object?>(null);
        }
    }

    internal class FailingJobStub<T>
    {
        private readonly T _result;
        private readonly int _fails;


        public int Attempts { get; internal set; }


        public FailingJobStub(int fails, T result)
        {
            _result = result;

            _fails = fails;
            this.Attempts = 0;
        }

        public async Task<T> ExecuteAsync(CancellationToken cancellationToken)
        {
            Debug.WriteLine("ExecuteAsync...");

            this.Attempts++;

            if (_fails >= this.Attempts)
            {
                throw new FailingException(this.Attempts, $"This is expected to fail {_fails} times and has failed {this.Attempts}.");
            }

            return await Task.FromResult<T>(_result);
        }
    }
}
