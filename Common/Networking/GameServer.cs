using System.Net;
using System.Net.Sockets;
using Common.Networking.Configuration;
using Common.Networking.Packets.Interfaces;

namespace Common.Networking;

public sealed class GameServer
{
    public readonly HashSet<GameClient> ConnectedPeers = new();
    private readonly TcpListener _listener;
    private readonly ServerConfig _serverConfig;
    private readonly IPacketProcessor _packetProcessor;

    public GameServer(ServerConfig serverConfig, IPacketProcessor packetProcessor)
    {
        _serverConfig = serverConfig;
        _packetProcessor = packetProcessor;
        _listener = new TcpListener(new IPEndPoint(IPAddress.Any, _serverConfig.ServerPort));
        _listener.Start();
        Console.WriteLine($"Started Listening For Connections on port {_serverConfig.ServerPort}.");
        _listener.BeginAcceptSocket(OnSocketAccepted, default);
    }

    private void OnSocketAccepted(IAsyncResult state)
    {
        try
        {
            var socket = _listener.EndAcceptSocket(state);
            var client = new GameClient(socket, _serverConfig);
            client.OnMessage += OnClientMessage;
            ConnectedPeers.Add(client);
            Console.WriteLine($"Received connection from: {socket.RemoteEndPoint}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Socket Connectivity Error: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
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