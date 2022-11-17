using System.Net;
using System.Net.Sockets;
using Common.Networking.Extensions;

namespace Common.Networking.Models;

public abstract class SocketClient
{
    public event Action<SocketClient>? OnDisconnection;
    public event Action<SocketClient, byte[]>? OnPayloadSent;
    public event Action<SocketClient, byte[]>? OnPayloadReceived;

    public IPAddress RemoteIpAddress { get; }
    public byte[] ReceiveIv { get; }
    public byte[] SendIv { get; }

    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly AutoResetEvent _autoResetEvent = new(false);
    private readonly Socket _socket;

    protected SocketClient(Socket socket)
    {
        _socket = socket;

        RemoteIpAddress = socket.GetRemoteIpAddress();
        ReceiveIv = ByteExtensions.GenerateRandomByteArray(4);
        SendIv = ByteExtensions.GenerateRandomByteArray(4);

        StartReceivingPackets();
    }

    private void StartReceivingPackets()
    {
        Task.Run(() =>
        {
            while (_socket.Connected && !_cancellationTokenSource.IsCancellationRequested)
            {
                var buffer = new byte[2048]; // prob set to max capacity we dont really care
                _socket.BeginReceive(buffer, 0, _socket.Available, 0, (IAsyncResult ar) =>
                {
                    OnPayloadReceived?.Invoke(this, buffer);
                    _autoResetEvent.Set();
                }, null);
                _autoResetEvent.WaitOne();
            }
        });
    }

    public virtual async ValueTask DisconnectAsync(CancellationToken cancellationToken = default)
    {
        await _socket.DisconnectAsync(false, cancellationToken);
        OnDisconnection?.Invoke(this);
    }

    public virtual async ValueTask SendPayloadAsync(byte[] payload, CancellationToken cancellationToken = default)
    {
        OnPayloadSent?.Invoke(this, payload);
        await _socket.SendAsync(payload, SocketFlags.None);
    }
}