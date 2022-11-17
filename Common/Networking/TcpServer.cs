using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Common.Networking.Models;
using Common.Networking.OperationCodes;

namespace Common.Networking;

public class TcpServer
{
    public ConcurrentDictionary<IPAddress, TcpGameClient> ConnectedPeers = new();

    private readonly TcpListener _listener;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly AutoResetEvent _autoResetEvent = new(false);

    public TcpServer(int port)
    {
        _listener = new TcpListener(IPAddress.Any, port);
    }

    public virtual void StartListeningForConnections()
    {
        Console.WriteLine("Started Listening For Connections.");
        _listener.Start();
    }

    public virtual async ValueTask ShutdownAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Shutting down.");
        _listener.Stop();

        _cancellationTokenSource.Cancel();

        var peerDisconnectionTasks = ConnectedPeers.Values.Select(DisconnectPeerAsync);

        await Task.WhenAll(peerDisconnectionTasks);

        ConnectedPeers.Clear();

        async Task DisconnectPeerAsync(SocketClient peerClient)
        {
            await peerClient.DisconnectAsync(cancellationToken);
        }
    }

    public void StartAcceptingConnections()
    {
        Console.WriteLine("Started Accepting Connections.");
        Task.Run(() =>
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                _listener.BeginAcceptSocket(OnSocketAccepted, default);
                _autoResetEvent.WaitOne();
            }
        });
    }

    private async void OnSocketAccepted(IAsyncResult state)
    {
        var clientSocket = _listener.EndAcceptSocket(state);
        var tcpGameClient = new TcpGameClient(clientSocket);

        // if (ConnectedPeers.ContainsKey(tcpGameClient.RemoteIpAddress))
        //     throw new InvalidOperationException();

        if (ConnectedPeers.ContainsKey(tcpGameClient.RemoteIpAddress))
            ConnectedPeers.TryRemove(tcpGameClient.RemoteIpAddress, out _);

        tcpGameClient.OnDisconnection += ClientDisconnected;
        tcpGameClient.OnPayloadSent += PayloadSent;
        tcpGameClient.OnPayloadReceived += PayloadReceived;

        ConnectedPeers.TryAdd(tcpGameClient.RemoteIpAddress, tcpGameClient);

        Console.WriteLine($"Received connection from: {tcpGameClient.RemoteIpAddress}");

        await SendHandshakePacket(tcpGameClient);

        _autoResetEvent.Set();
    }

    private void PayloadReceived(SocketClient socketClient, byte[] payload)
    {
        Console.WriteLine($"Received a payload from client #{socketClient.RemoteIpAddress} : {BitConverter.ToString(payload)}");
    }

    private void PayloadSent(SocketClient socketClient, byte[] payload)
    {
        Console.WriteLine($"Sent payload to client #{socketClient.RemoteIpAddress} : {BitConverter.ToString(payload)}");
    }

    private void ClientDisconnected(SocketClient socketClient)
    {
        ConnectedPeers.TryRemove(socketClient.RemoteIpAddress, out _);

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