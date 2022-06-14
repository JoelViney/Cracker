
namespace Cracker
{
    [TestClass]
    public class ThrottleTests
    {
        [TestMethod]
        public async Task ExecuteWasCalled()
        {
            // Arrange
            var job = new SimpleJobStub();

            // Act
            await new CrackerBuilder()
                .Throttle(callLimit: 1, period: TimeSpan.FromSeconds(1))
                .ExecuteAsync(job.ExecuteAsync);

            // Assert
            Assert.IsTrue(job.Completed);
        }

        [TestMethod]
        public async Task ThrottlingLimits()
        {
            // Arrange
            var job = new SimpleJobStub();
            var tickStart = DateTime.UtcNow.Ticks;

            var builder = new CrackerBuilder()
                .Throttle(callLimit: 1, period: TimeSpan.FromSeconds(1));

            var timer = new TestTimer();

            // Act
            await builder.ExecuteAsync(job.ExecuteAsync);
            await builder.ExecuteAsync(job.ExecuteAsync);

            // Assert
            Assert.IsTrue(timer.TotalMilliseconds >= 100);
        }
    }
}
