namespace LongRunningJob.API.Services;

public class DelayService : IDelayService
{
    private readonly int _minMilliseconds;
    private readonly int _maxMilliseconds;

    public DelayService(IConfiguration configuration)
    {
        _minMilliseconds = configuration.GetValue<int>("Delay:MinMilliseconds");
        _maxMilliseconds = configuration.GetValue<int>("Delay:MaxMilliseconds");
    }

    public Task DelayAsync(int milliseconds, CancellationToken cancellationToken)
    {
        return Task.Delay(milliseconds, cancellationToken);
    }

    public Task RandomDelayAsync(int minMilliseconds, int maxMilliseconds, CancellationToken cancellationToken)
    {
        var delay = new Random().Next(minMilliseconds, maxMilliseconds);
        return DelayAsync(delay, cancellationToken);
    }

    public Task RandomDelayAsync(CancellationToken cancellationToken)
    {
        return RandomDelayAsync(_minMilliseconds, _maxMilliseconds, cancellationToken);
    }
}