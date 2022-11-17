using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Common.Networking.Models;
using Common.Networking.OperationCodes;

namespace Common.Networking;

public class TcpServer
{
    public readonly HashSet<SocketClient> ConnectedPeers = new();

    private readonly TcpListener _listener;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly AutoResetEvent _autoResetEvent = new(false);

    public TcpServer(int port)
    {
        _listener = new TcpListener(IPAddress.Any, port);
        _listener.Start();
        Console.WriteLine("Started Listening For Connections.");
        BeginAcceptingSockets();
    }

    // probably redundant to disconnect clients, they get dc'd anyways
    public virtual async ValueTask ShutdownAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Shutting down.");
        _listener.Stop();

        _cancellationTokenSource.Cancel();

        var peerDisconnectionTasks = ConnectedPeers.Select(DisconnectPeerAsync);

        await Task.WhenAll(peerDisconnectionTasks);

        ConnectedPeers.Clear();

        async Task DisconnectPeerAsync(SocketClient peerClient)
        {
            await peerClient.DisconnectAsync(cancellationToken);
        }
    }

    private void BeginAcceptingSockets()
    {
        _listener.BeginAcceptSocket(OnSocketAccepted, default);
    }

    private async void OnSocketAccepted(IAsyncResult state)
    {
        var clientSocket = _listener.EndAcceptSocket(state);
        var tcpGameClient = new TcpGameClient(clientSocket);

        BeginAcceptingSockets();

        tcpGameClient.OnDisconnection += ClientDisconnected;
        tcpGameClient.OnPayloadSent += PayloadSent;
        tcpGameClient.OnPayloadReceived += PayloadReceived;

        ConnectedPeers.Add(tcpGameClient);

        Console.WriteLine($"Received connection from: {tcpGameClient.RemoteIpAddress}");

        await SendHandshakePacket(tcpGameClient);
    }

    private void PayloadReceived(SocketClient socketClient, byte[] payload)
    {
        /*
         * short - packet length
         * short - packet header
         * 
         */
        using var reader = new ByteBuffer(payload);
        var packetLength = reader.ReadShort();

        if (packetLength == 0)
            return;

        Console.WriteLine($"Received a payload from client #{socketClient.RemoteIpAddress} : {BitConverter.ToString(payload)}");
    }

    private void PayloadSent(SocketClient socketClient, byte[] payload)
    {
        Console.WriteLine($"Sent payload to client #{socketClient.RemoteIpAddress} : {BitConverter.ToString(payload)}");
    }

    private void ClientDisconnected(SocketClient socketClient)
    {
        ConnectedPeers.Remove(socketClient);

        Console.WriteLine($"Client disconnection: {socketClient.RemoteIpAddress}");
    }

    private async ValueTask SendHandshakePacket(SocketClient socketClient)
    {
        using var payload = new ByteBuffer(EServerOperationCode.Handshake);
        payload.WriteShort(76);
        payload.WriteString("1");
        payload.WriteBytes(socketClient.ReceiveIv);
        payload.WriteBytes(socketClient.SendIv);
        payload.WriteByte(9);
        await socketClient.SendPayloadAsync(payload.GetTrimmedPacket());
    }
}