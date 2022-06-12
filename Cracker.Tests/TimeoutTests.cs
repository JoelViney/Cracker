
namespace Cracker
{
    [TestClass]
    public class TimeoutTests
    {
        [TestMethod]
        public async Task TimeoutBeforeJobFinished()
        {
            // Arrange
            var job = new SlowJobStub(milliseconds: 1000);

            // Act
            await new CrackerBuilder()
                .Timeout(20)
                .ExecuteAsync(job.ExecuteAsync);

            // Assert
            Assert.IsFalse(job.Completed, "Failed to cancel on timeout.");
        }

        // Task takes 20ms, timeout is 100ms so task will complete
        [TestMethod]
        public async Task CompleteJobBeforeTimeout()
        {
            // Arrange
            var job = new SlowJobStub(milliseconds: 20);

            // Act
            await new CrackerBuilder()
                .Timeout(100)
                .ExecuteAsync(job.ExecuteAsync);

            // Assert
            Assert.IsTrue(job.Completed, "Cancel on timeout when not expected.");
        }

        // Task takes 100ms, timeout is 20ms so the task will timeout and not complete
        [TestMethod]
        public async Task TimeoutBeforeJobFinishedWithResult()
        {
            // Arrange
            var job = new SlowJobStub(milliseconds: 100);

            // Act
            await new CrackerBuilder()
                .Timeout(20)
                .ExecuteAsync(job.ExecuteAsync);

            // Assert
            Assert.IsFalse(job.Completed, "Failed to cancel on timeout.");
        }
    }
}
