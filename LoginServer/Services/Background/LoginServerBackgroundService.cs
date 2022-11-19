using System.Net;
using Common.Networking;
using Common.Networking.Configuration;
using Common.Networking.Packets.Interfaces;
using LoginServer.Configuration;
using Microsoft.Extensions.Options;

namespace LoginServer.Services.Background;

public sealed class LoginServerBackgroundService : IHostedService
{
    private readonly GameServer _gameServer;

    public LoginServerBackgroundService(IPacketProcessor packetProcessor, IOptions<ServerConfig> serverConfig)
    {
        _gameServer = new GameServer(serverConfig.Value, packetProcessor);
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}