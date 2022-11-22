using Common.Enums;
using Common.Networking;
using Common.Networking.Configuration;
using Microsoft.Extensions.Options;

namespace ChannelServer.Services.Background;

public sealed class ChannelServerBackgroundService : IHostedService
{
    private readonly GameServer _gameServer;
    private readonly ILogger<ChannelServerBackgroundService> _logger;
    private readonly ServerConfig _serverConfig;

    public ChannelServerBackgroundService(
        GameServer gameServer,
        ILogger<ChannelServerBackgroundService> logger,
        IOptions<ServerConfig> serverConfig)
    {
        _gameServer = gameServer;
        _logger = logger;
        _serverConfig = serverConfig.Value;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Channel Server Information: v{_serverConfig.ClientVersion}.{_serverConfig.ClientPatchVersion} [{Enum.GetName(typeof(EClientLocale), _serverConfig.ClientLocale)}]");
        _gameServer.Start();
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Shutting down the server..");
        await _gameServer.DisposeAsync();
    }
}