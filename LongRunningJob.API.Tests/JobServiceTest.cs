using LongRunningJob.API.Services;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using LongRunningJob.API.Hubs;

namespace LongRunningJob.API.Tests;

public class JobServiceTest
{
    [Fact]
    public void ConstructorTest()
    {
        // Arrange
        var logger = new Mock<ILogger<JobService>>();
        var delaySvcs = new Mock<IDelayService>();
        var hubContext = new Mock<IHubContext<ProcessingHub>>();

        var jobService = new JobService(hubContext.Object, delaySvcs.Object, logger.Object);

        // Act

        // Assert
        Assert.NotNull(jobService);
    }

    [Fact]
    public async Task StartJobAsyncTest()
    {
        // Arrange
        var connectionId = "connectionId";
        var logger = new Mock<ILogger<JobService>>();
        var delaySvcs = new Mock<IDelayService>();
        delaySvcs.Setup(x => x.RandomDelayAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var hubContext = new Mock<IHubContext<ProcessingHub>>();
        var clients = new Mock<IHubClients>();
        var clientProxy = new Mock<ISingleClientProxy>();
        clientProxy.Setup(x => x.SendCoreAsync("Character", It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        clientProxy.Setup(x => x.SendCoreAsync("Progress", It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        clients.Setup(x => x.Client(connectionId)).Returns(clientProxy.Object);
        hubContext.Setup(x => x.Clients).Returns(clients.Object);
        
        var jobService = new JobService(hubContext.Object, delaySvcs.Object, logger.Object);

        // Act
        var result = await jobService.StartJobAsync(connectionId, "input", CancellationToken.None);

        // Assert
        Assert.True(result != Guid.Empty);
    }
}