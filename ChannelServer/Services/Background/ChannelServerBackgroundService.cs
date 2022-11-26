using ChannelServer.Services.Interfaces;

namespace ChannelServer.Services.Background;

public sealed class ChannelServerBackgroundService : IHostedService
{
    private readonly IChannelServer _channelServer;
    private readonly ILogger<ChannelServerBackgroundService> _logger;

    public ChannelServerBackgroundService(
        IChannelServer channelServer,
        ILogger<ChannelServerBackgroundService> logger)
    {
        _channelServer = channelServer;
        _logger = logger;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _channelServer.Start();
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _channelServer.DisposeAsync();
    }
}