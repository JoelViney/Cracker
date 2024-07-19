
# Cracker

I saw some [Polly](https://github.com/App-vNext/Polly) code in an application and wondered why it was so complex... 

I had previously written my own [throttle and retry](https://github.com/JoelViney/XeroThrottleAndRetry) code for Xero so I thought I would try rolling my own version with Timeout for fun.

I deliberately didnt look at the Polly source code as it would throw my implementation.

Example:

```
var result = await new CrackerBuilder()
    .Throttle(callLimit: 100, period: TimeSpan.FromSeconds(60))
    .Timeout(TimeSpan.FromSeconds(1))
    .Retry(retryAttempts: 3)
    .WithDelayBetweenRetries(attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)))
    .UnessException<CustomException>()
    .UnessException<HttpException>(x => x.ErrorCode = 404)
    .ExecuteAsync(job.ExecuteAsync);
```

TODO:
* Timeout applies to the entire call stack, it should only apply for the calls after it on the bulder.
e.g.
```
// Timeout after 1 second, attempt up to 3 times, .
var result = await new CrackerBuilder()
    .Timeout(TimeSpan.FromSeconds(1))
    .Retry(retryAttempts: 3)
    .ExecuteAsync(job.ExecuteAsync);

// vrs
// Attempt 3 times, timeout after 1 second on each try.
var result = await new CrackerBuilder()
    .Retry(retryAttempts: 3)
    .Timeout(TimeSpan.FromSeconds(1))
    .ExecuteAsync(job.ExecuteAsync);
```
