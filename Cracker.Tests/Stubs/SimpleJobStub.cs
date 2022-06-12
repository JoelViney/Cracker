using System.Diagnostics;

namespace Cracker.Stubs
{
    internal class SimpleJobStub
    {
        public bool Completed { get; internal set; } = false;

        public SimpleJobStub()
        {
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Debug.WriteLine("ExecuteAsync...");

            this.Completed = true;
            await Task.FromResult<object?>(null);
        }
    }

    internal class SimpleJobStub<T>
    {
        private readonly T _result;
        private readonly List<T> _results;
        public int Attempts { get; internal set; }

        public SimpleJobStub(T result)
        {
            _result = result;
            _results = new List<T>();
        }

        public SimpleJobStub(T[] results)
        {
            _result = results.Last();
            _results = results.ToList();
        }

        public async Task<T> ExecuteAsync(CancellationToken cancellationToken)
        {
            Debug.WriteLine("ExecuteAsync...");

            this.Attempts++;

            if (_results != null && _results.Count >= this.Attempts)
            {
                var result = _results[this.Attempts - 1];
                return await Task.FromResult<T>(result);
            }

            return await Task.FromResult<T>(_result);
        }
    }
}
