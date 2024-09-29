namespace LongRunningJob.API.Services;

public interface IDelayService
{
    Task DelayAsync(int milliseconds, CancellationToken cancellationToken);
    Task RandomDelayAsync(int minMilliseconds, int maxMilliseconds, CancellationToken cancellationToken);
    Task RandomDelayAsync(CancellationToken cancellationToken);
}