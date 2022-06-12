
namespace Cracker.Helpers
{
    internal class TestTimer
    {
        private readonly long _tickStart;

        public TestTimer()
        {
            _tickStart = DateTime.UtcNow.Ticks;
        }

        public double TotalMilliseconds
        {
            get
            {
                long tickEnd = DateTime.UtcNow.Ticks;
                TimeSpan timeSpan = TimeSpan.FromTicks(tickEnd - _tickStart);
                var duration = timeSpan.Duration().TotalMilliseconds;

                return duration;
            }
        }
    }
}
