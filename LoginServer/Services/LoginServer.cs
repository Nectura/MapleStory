using Common.Enums;
using Common.Networking;
using Common.Networking.Abstract;
using Common.Networking.Configuration;
using Common.Networking.Packets.Interfaces;
using LoginServer.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace LoginServer.Services;

public sealed class LoginServer : GameServer, ILoginServer
{
    public HashSet<GameClient> ConnectedPeers { get; init; } = new();

    public LoginServer(
        IOptions<ServerConfig> serverConfig,
        IPacketProcessor packetProcessor,
        ILogger<LoginServer> logger) : base(serverConfig, packetProcessor, logger)
    {
    }
    
    public override async ValueTask DisposeAsync()
    {
        _logger.LogInformation("Shutting down the server..");
        
        await base.DisposeAsync();
        await Task.WhenAll(ConnectedPeers.Select(m => m.DisconnectAsync()));
        
        ConnectedPeers.Clear();
    }
    
    public override void Start()
    {
        base.Start();
        
        _logger.LogInformation("Login Server Information: v{clientVer}.{clientPatchVer} [{locale}]",
            _serverConfig.ClientVersion,
            _serverConfig.ClientPatchVersion,
            Enum.GetName(typeof(EClientLocale), _serverConfig.ClientLocale));
        
        _logger.LogInformation("Started Listening For Connections on port {serverPort}.", 
            _serverConfig.ServerPort);
    }

    protected override void HandleClientDisconnection(GameClient client)
    {
        ConnectedPeers.Remove(client);
        base.HandleClientDisconnection(client);
    }

    protected override void HandleClientConnection(GameClient client)
    {
        ConnectedPeers.Add(client);
        base.HandleClientConnection(client);
    }
}