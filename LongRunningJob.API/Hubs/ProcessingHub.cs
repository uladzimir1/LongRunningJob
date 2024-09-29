using LongRunningJob.API.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace LongRunningJob.API.Hubs
{
    public class ProcessingHub : Hub
    {
        private readonly IJobService _jobService;
        private readonly ILogger<ProcessingHub> _logger;

        public ProcessingHub(IJobService jobService, ILogger<ProcessingHub> logger)
        {
            _jobService = jobService;
            _logger = logger; 
        }

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }

        public Task StartProcessing(string input)
        {
            _jobService.StartJobAsync(Context.ConnectionId, input);

            return Task.CompletedTask;
        }

        public async Task CancelProcessing()
        {
            await _jobService.CancelJobAsync(Context.ConnectionId);
        }
    }
}