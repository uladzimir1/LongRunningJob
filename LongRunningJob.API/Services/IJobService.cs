using System;

namespace LongRunningJob.API.Services;

public interface IJobService
{
    Task<Guid> StartJobAsync(string connectionId, string input, CancellationToken cancellationToken = default);
    Task CancelJobAsync(string connectionId, CancellationToken cancellationToken = default);
}