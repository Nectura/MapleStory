using System.Net;
using Common.Networking;
using Common.Networking.Packets.Services;

namespace LoginServer.Services.Background;

public sealed class LoginServerBackgroundService : IHostedService
{
    private readonly GameServer _gameServer;

    public LoginServerBackgroundService(PacketProcessor packetProcessor)
    {
        _gameServer = new GameServer(new IPEndPoint(IPAddress.Any, 8484), packetProcessor);
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