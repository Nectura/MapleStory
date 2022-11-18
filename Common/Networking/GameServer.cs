using System.Net;
using System.Net.Sockets;
using Common.Networking.Services;

namespace Common.Networking;

public sealed class GameServer
{
    public readonly HashSet<GameClient> ConnectedPeers = new();
    private readonly TcpListener _listener;
    private readonly PacketProcessor _packetProcessor;

    public GameServer(IPEndPoint endpoint, PacketProcessor packetProcessor)
    {
        _packetProcessor = packetProcessor;
        _listener = new TcpListener(endpoint);
        _listener.Start();
        Console.WriteLine("Started Listening For Connections.");
        _listener.BeginAcceptSocket(OnSocketAccepted, default);
    }

    private void OnSocketAccepted(IAsyncResult state)
    {
        try
        {
            var socket = _listener.EndAcceptSocket(state);
            var client = new GameClient(socket);
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