
# Cracker

I saw some [Polly](https://github.com/App-vNext/Polly) code in an application and wondered why it was so complex...

I had previously wirten my own [throttle and retry](https://github.com/JoelViney/XeroThrottleAndRetry) code for Xero so I thought I would try rolling my own version with Timeout for fun.

I deliberatly didnt look at the Polly source code as it would throw my implementation.


```
var result = await new CrackerBuilder()
    .Throttle(callLimit: 60, timePeriodMilliseconds: 60000)
    .Timeout(1000)
    .Retry(attempts: 3, retryIntervalMilliseconds: 100)
    .UnessException<CustomException>()
    .UnessException<CustomException>()
    .ExecuteAsync(job.ExecuteAsync);
```

TODO:
* Timeout applies to the entire call stack, it should only apply for the calls after it on the bulder.
* I should add TimeSpans as they are more readable with big numbers.
* I should add a retry durations instead of it being hard coded.
* Circuit-Breaker? I get it, but its more a job concept.
* There is prolly a whole lot more stuff in Polly that I havn't implemented.