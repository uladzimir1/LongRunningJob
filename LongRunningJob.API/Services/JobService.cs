using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LongRunningJob.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace LongRunningJob.API.Services;

public class JobService : IJobService
{
    private readonly ILogger<JobService> _logger;
    private readonly IDelayService _delaySvc;
    private readonly IHubContext<ProcessingHub> _processingHub;
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _ctsDictionary = new();
    private readonly ConcurrentDictionary<string, Task> _processingTasks = new();

 
    public JobService(IHubContext<ProcessingHub> processingHub, IDelayService delaySvc, ILogger<JobService> logger)
    {
        _logger = logger;
        _delaySvc = delaySvc;
        _processingHub = processingHub;
    }

    public Task<Guid> StartJobAsync(string connectionId, string input, CancellationToken cancellationToken)
    {
        var cts = new CancellationTokenSource();
        _ctsDictionary.TryAdd(connectionId, cts);

        var jobId = Guid.NewGuid();
        var task = Task.Run(() => ProcessStringAsync(connectionId, input, jobId, cts.Token), cts.Token);
        _processingTasks.TryAdd(connectionId, task);

        return Task.FromResult(jobId);
    }

    private async Task ProcessStringAsync(string connectionId, string input, Guid jobId, CancellationToken token)
    {
        var outputString = GenerateOutputString(input);
        var progress = 0;
        _logger.LogInformation("Job {jobId} has been started. Connection ID: {connectionId}, Input: {input}, Output: {outputString}", jobId, connectionId, input, outputString);
        await _processingHub.Clients.Client(connectionId).SendAsync("Started", jobId, token);

        try
        {
            foreach (var character in outputString)
            {
                if (token.IsCancellationRequested)
                {
                    _logger.LogInformation("Job {jobId} has been cancelled. Connection ID: {connectionId}", jobId, connectionId);
                    break;
                }

                await _delaySvc.RandomDelayAsync(token);
                await _processingHub.Clients.Client(connectionId).SendAsync("Character", character);

                progress++;
                await _processingHub.Clients.Client(connectionId).SendAsync(
                    "Progress",
                    new { Progress = progress, Overall = outputString.Length });

                _logger
                    .LogInformation("Job {jobId} has been updated (Progress {progress}/{outputString.Length}). Connection ID: {connectionId}"
                        , jobId, progress, outputString.Length, connectionId);
            }
            await _processingHub.Clients.Client(connectionId).SendAsync("Completed", jobId);
        }
        catch (OperationCanceledException ex)
        {
            _logger
                .LogInformation("Job {jobId} has been cancelled. Connection ID: {connectionId}, Exception: {ex}",
                        jobId, connectionId, ex);
            await _processingHub.Clients.Client(connectionId).SendAsync("Cancelled", jobId);
        }
        finally
        {
            _ctsDictionary.TryRemove(connectionId, out var cts);
            cts?.Dispose();
            _processingTasks.TryRemove(connectionId, out var task);
            task?.Dispose();
        }
    }

    public Task CancelJobAsync(string connectionId, CancellationToken cancellationToken)
    {
        if (_ctsDictionary.TryGetValue(connectionId, out var cts))
        {
            cts.Cancel();
            _logger.LogInformation("Job has been cancelled. Connection ID: {connectionId}", connectionId);
        }
        return Task.CompletedTask;
    }

    private string GenerateOutputString(string input)
    {
        // Count unique characters
        var charCounts = input.GroupBy(c => c)
                              .OrderBy(g => g.Key)
                              .Select(g => $"{g.Key}{g.Count()}");
        var countsString = string.Concat(charCounts);

        // Base64 encode
        var base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(input));

        return countsString + "/" + base64String;
    }
}