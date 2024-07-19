
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
        [TestCategory("NotImplemented")]
        [ExpectedException(typeof(TaskCanceledException))]
        public async Task TimeoutAfterRetry()
        {
            // Arrange
            var job = new SlowJobStub(milliseconds: 100);

            // Act
            try
            {
                await new CrackerBuilder()
                    .Retry(retryAttempts: 3)
                    .Timeout(20)
                    .ExecuteAsync(job.ExecuteAsync);
            }
            finally
            {
                // Assert
                Assert.IsFalse(job.Completed);
                Assert.AreEqual(3, job.Attempts);
            }
        }
    }
}