
namespace Cracker
{
    [TestClass]
    public class BuilderTests
    {
        [TestMethod]
        public async Task EmptyBuilderExecutes()
        {
            // Arrange
            var job = new SimpleJobStub();

            // Act
            await new CrackerBuilder()
                .ExecuteAsync(job.ExecuteAsync);

            // Assert
            Assert.IsTrue(job.Completed);
        }

        [TestMethod]
        public async Task EmptyBuilderReturnsResult()
        {
            // Arrange
            var job = new SimpleJobStub<Boolean>(true);

            // Act
            var result = await new CrackerBuilder()
                .ExecuteAsync(job.ExecuteAsync);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task TimeoutBeforeRetry()
        {
            // Arrange
            var job = new SlowJobStub(milliseconds: 100);

            // Act
            await new CrackerBuilder()
                .Timeout(20)
                .Retry(attempts: 3, retryIntervalMilliseconds: 1000)
                .ExecuteAsync(job.ExecuteAsync);

            // Assert
            Assert.IsFalse(job.Completed);
            Assert.AreEqual(1, job.Attempts);
        }
    }
}