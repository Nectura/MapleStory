using System.Net;
using System.Net.Sockets;
using Common.Networking.Abstract.Interfaces;
using Common.Networking.Configuration;
using Common.Networking.Packets.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.Networking.Abstract;

public abstract class GameServer : IAsyncDisposable, IGameServer
{
    public bool IsRunning => _listener.Server.Connected;
    
    protected readonly ServerConfig _serverConfig;
    protected readonly ILogger _logger;

    private readonly IPacketProcessor _packetProcessor;
    private readonly TcpListener _listener;
    private readonly IServiceScopeFactory _scopeFactory;

    protected GameServer(
        IOptions<ServerConfig> serverConfig,
        IPacketProcessor packetProcessor,
        ILogger logger,
        IServiceScopeFactory scopeFactory)
    {
        _serverConfig = serverConfig.Value;
        _packetProcessor = packetProcessor;
        _logger = logger;
        _scopeFactory = scopeFactory;
        _listener = new TcpListener(new IPEndPoint(IPAddress.Any, _serverConfig.ServerPort));
    }

    public virtual ValueTask DisposeAsync()
    {
        _listener.Stop();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
    
    public virtual void Start()
    {
        if (IsRunning) return;
        _listener.Start();
        _listener.BeginAcceptSocket(OnSocketAccepted, default);
    }

    protected virtual void HandleClientMessageReceived(GameClient client, GameMessageBuffer message)
    {
        _packetProcessor.ProcessPacket(client, message);
    }
    
    protected virtual void HandleClientDisconnection(GameClient client)
    {
        _logger.LogInformation("Client disconnected: {remoteAddress}", client.EndPoint?.ToString() ?? client.IpAddress.ToString());
    }

    protected virtual void HandleClientConnection(GameClient client)
    {
        _logger.LogInformation("Client connected: {remoteAddress}", client.EndPoint?.ToString() ?? client.IpAddress.ToString());
    }
    
    private void OnSocketAccepted(IAsyncResult state)
    {
        try
        {
            var socket = _listener.EndAcceptSocket(state);
            var client = new GameClient(socket, _serverConfig, _scopeFactory);
            client.OnMessageReceived += HandleClientMessageReceived;
            client.OnDisconnected += HandleClientDisconnection;
            HandleClientConnection(client);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to accept a socket connection: {exceptionMsg}", $"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
        }
        finally
        {
            _listener.BeginAcceptSocket(OnSocketAccepted, default);
        }
    }
}