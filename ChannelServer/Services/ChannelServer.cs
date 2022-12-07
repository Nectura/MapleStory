using ChannelServer.Services.Interfaces;
using Common.Enums;
using Common.Networking;
using Common.Networking.Abstract;
using Common.Networking.Configuration;
using Common.Networking.Packets.Interfaces;
using Microsoft.Extensions.Options;

namespace ChannelServer.Services;

public sealed class ChannelServer : GameServer, IChannelServer
{
    public Dictionary<uint, GameClient> ConnectedPeers { get; init; } = new();

    public ChannelServer(
        IOptions<ServerConfig> serverConfig,
        IPacketProcessor packetProcessor,
        ILogger<ChannelServer> logger,
        IServiceScopeFactory scopeFactory) : base(serverConfig, packetProcessor, logger, scopeFactory)
    {
    }
    
    public override async ValueTask DisposeAsync()
    {
        _logger.LogInformation("Shutting down the server..");
        
        await base.DisposeAsync();
        await Task.WhenAll(ConnectedPeers.Values.Select(m => m.DisconnectAsync()));
        
        ConnectedPeers.Clear();
    }

    public override void Start()
    {
        base.Start();
        
        _logger.LogInformation("Channel Server Information: v{clientVer}.{clientPatchVer} [{locale}]",
            _serverConfig.ClientVersion,
            _serverConfig.ClientPatchVersion,
            Enum.GetName(typeof(EClientLocale), _serverConfig.ClientLocale));
        
        _logger.LogInformation("Started Listening For Connections on port {serverPort}.", 
            _serverConfig.ServerPort);
    }

    protected override void HandleClientDisconnection(GameClient client)
    {
        ConnectedPeers.Remove(client.Account.Id, out _);
        base.HandleClientDisconnection(client);
    }
}