
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
                .Throttle(callLimit: 1, timePeriodMilliseconds: 1000)
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
                .Throttle(callLimit: 1, timePeriodMilliseconds: 100);

            var timer = new TestTimer();

            // Act
            await builder.ExecuteAsync(job.ExecuteAsync);
            await builder.ExecuteAsync(job.ExecuteAsync);

            // Assert
            Assert.IsTrue(timer.TotalMilliseconds >= 100);
        }
    }
}
