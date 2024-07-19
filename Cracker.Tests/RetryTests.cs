
namespace Cracker
{
    [TestClass]
    public class RetryTests
    {
        [TestMethod]
        public async Task Retry()
        {
            // Arrange
            var job = new FailingJobStub(fails: 1);

            // Act
            await new CrackerBuilder()
                .Retry(2)
                .ExecuteAsync(job.ExecuteAsync);

            // Assert
            Assert.AreEqual(2, job.Attempts);
            Assert.IsTrue(job.Completed);
        }

        [TestMethod]
        public async Task RetryWithReturnValue()
        {
            // Arrange
            var job = new FailingJobStub<Boolean>(fails: 1, result: true);

            // Act
            var result = await new CrackerBuilder()
                .Retry(2)
                .ExecuteAsync<Boolean>(job.ExecuteAsync);

            // Assert
            Assert.AreEqual(2, job.Attempts);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task RetryNotNeeded()
        {
            // Arrange
            var job = new FailingJobStub(fails: 0);

            // Act
            await new CrackerBuilder()
                .Retry(3)
                .ExecuteAsync(job.ExecuteAsync);

            // Assert
            Assert.AreEqual(1, job.Attempts);
            Assert.IsTrue(job.Completed);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))] // Failing and TimeoutException
        public async Task RetryAttemptsExceeded()
        {
            // Arrange
            var job = new FailingJobStub(fails: 100);

            // Act
            try
            {
                await new CrackerBuilder()
                    .Retry(1)
                    .ExecuteAsync(job.ExecuteAsync);
            }
            catch
            {
                // Assert - Expecting an exception
                Assert.AreEqual(2, job.Attempts);
                throw;
            } 
        }

        [TestMethod]
        [ExpectedException(typeof(FailingException))]
        public async Task DontRetryOnSpecificException()
        {
            // Arrange
            var job = new FailingJobStub(fails: 1);

            // Act
            await new CrackerBuilder()
                .Retry(2)
                .UnlessException<FailingException>()
                .ExecuteAsync(job.ExecuteAsync);
            
            // Assert - Expecting an exception
        }

        [TestMethod]
        public async Task IgnoreInvalidExceptionPredicate()
        {
            // Arrange
            var job = new FailingJobStub(fails: 1);

            // Act
            await new CrackerBuilder()
                .Retry(2)
                .UnlessException<FailingException>(x => x.Attempts == 100) // This wont happen
                .ExecuteAsync(job.ExecuteAsync);

            // Assert
            Assert.IsTrue(job.Completed);
        }

        [TestMethod]
        [ExpectedException(typeof(FailingException))]
        public async Task FailOnMatchingExceptionPredicate()
        {
            // Arrange
            var job = new FailingJobStub(fails: 1);

            // Act
            await new CrackerBuilder()
                .Retry(2)
                .UnlessException<FailingException>(x => x.Attempts == 1) // This will happen
                .ExecuteAsync(job.ExecuteAsync);

            // Assert - Expecting an exception
        }

        [TestMethod]
        [ExpectedException(typeof(FailingException))]
        public async Task FailOnSecondMatchingExceptionPredicate()
        {
            // Arrange
            var job = new FailingJobStub(fails: 2);

            // Act
            await new CrackerBuilder()
                .Retry(2)
                .WhenException<FailingException>(x => x.Attempts == 1)      // This will happen
                .UnlessException<FailingException>(x => x.Attempts == 2)    // This will happen
                .ExecuteAsync(job.ExecuteAsync);

            // Assert - Expecting an exception
        }

        [TestMethod]
        public async Task FailOnSpecificException()
        {
            // Arrange
            var job = new FailingJobStub(fails: 1);

            // Act
            await new CrackerBuilder()
                .Retry(1)
                .WhenException<FailingException>()
                .ExecuteAsync(job.ExecuteAsync);

            // Assert
            Assert.AreEqual(2, job.Attempts);
        }

        [TestMethod]
        public async Task RetryWhenResult()
        {
            // Arrange
            var job = new SimpleJobStub<Boolean>(new bool[] { false, true });

            // Act
            var result = await new CrackerBuilder()
                .Retry(2)
                .WhenResult<Boolean>(x => x == false)
                .ExecuteAsync(job.ExecuteAsync);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
