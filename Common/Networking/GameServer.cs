using System.Net;
using System.Net.Sockets;
using Common.Networking.Configuration;
using Common.Networking.Packets.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.Networking;

public sealed class GameServer : IAsyncDisposable
{
    public readonly HashSet<GameClient> ConnectedPeers = new();

    public bool IsRunning => _listener.Server.Connected;
    
    private readonly ServerConfig _serverConfig;
    private readonly IPacketProcessor _packetProcessor;
    private readonly ILogger<GameServer> _logger;
    
    private readonly TcpListener _listener;

    public GameServer(
        IOptions<ServerConfig> serverConfig,
        IPacketProcessor packetProcessor,
        ILogger<GameServer> logger)
    {
        _serverConfig = serverConfig.Value;
        _packetProcessor = packetProcessor;
        _logger = logger;
        
        _listener = new TcpListener(new IPEndPoint(IPAddress.Any, _serverConfig.ServerPort));
    }

    public void Start()
    {
        if (IsRunning) return;
        _listener.Start();
        _listener.BeginAcceptSocket(OnSocketAccepted, default);
        _logger.LogInformation($"Started Listening For Connections on port {_serverConfig.ServerPort}.");
    }
    
    public async ValueTask DisposeAsync()
    {
        _listener.Stop();
        var disconnectionTasks = ConnectedPeers.Select(m => m.DisconnectAsync()).ToArray();
        await Task.WhenAll(disconnectionTasks);
    }

    private void OnSocketAccepted(IAsyncResult state)
    {
        try
        {
            var socket = _listener.EndAcceptSocket(state);
            var client = new GameClient(socket, _serverConfig);
            client.OnMessage += OnClientMessage;
            ConnectedPeers.Add(client);
            _logger.LogInformation($"Received connection from: {socket.RemoteEndPoint}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Connection Issue: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
        }
        finally
        {
            _listener.BeginAcceptSocket(OnSocketAccepted, default);
        }
    }

    private void OnClientMessage(GameClient client, GameMessageBuffer message)
    {
        _packetProcessor.ProcessPacket(client, message);
    }
}