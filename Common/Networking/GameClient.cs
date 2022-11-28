using System.Net;
using Common.Networking.Cryptography;
using System.Net.Sockets;
using Common.Database.Models.Interfaces;
using Common.Enums;
using Common.Networking.Configuration;
using Common.Networking.Extensions;
using Common.Networking.Packets.Enums;

namespace Common.Networking;

public sealed class GameClient
{
    private readonly byte[] _buffer = new byte[4096];
    private int _size = 0;
    private readonly Socket _socket;
    private readonly ServerConfig _serverConfig;
    private readonly MapleIV _sendVector, _recvVector;

    public event Action<GameClient, GameMessageBuffer>? OnMessageReceived;
    public event Action<GameClient>? OnDisconnected;

    public IPAddress IpAddress => _socket.GetRemoteIpAddress();
    public EndPoint? EndPoint => _socket.RemoteEndPoint;

    public IAccount? Account { get; set; }
    public ICharacter? Character { get; set; }
    public EWorld World { get; set; }
    public byte Channel { get; set; }

    public GameClient(Socket socket, ServerConfig serverConfig)
    {
        _socket = socket;
        _serverConfig = serverConfig;
        _sendVector = new MapleIV((uint)Random.Shared.Next());
        _recvVector = new MapleIV((uint)Random.Shared.Next());
        
        SendRaw(new GameMessageBuffer(EServerOperationCode.Handshake)
            .WriteUShort(_serverConfig.ClientVersion)
            .WriteString(_serverConfig.ClientPatchVersion)
            .WriteUInt(_recvVector.Value)
            .WriteUInt(_sendVector.Value)
            .WriteByte((byte)_serverConfig.ClientLocale));
        
        socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, out SocketError errorCode, OnReceive, null);
        // TODO errorCode
    }

    private void OnReceive(IAsyncResult ar)
    {
        _size += _socket.EndReceive(ar, out SocketError errorCode);
        // TODO errorCode
        if (_size < 4)
            return;
        int length = (_buffer[0] + (_buffer[1] << 8)) ^ (_buffer[2] + (_buffer[3] << 8));
        if (_size < 4 + length)
            return;
        byte[] payload = new byte[length];
        Buffer.BlockCopy(_buffer, 4, payload, 0, length);
        _size -= length + 4;
        Buffer.BlockCopy(_buffer, 4, _buffer, 0, _size);
        MapleAES.Transform(payload, _recvVector);
        Shanda.DecryptTransform(payload);
        OnMessageReceived?.Invoke(this, new GameMessageBuffer(payload));

        _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, out SocketError errorCode2, OnReceive, null);
    }

    public void Send(GameMessageBuffer buffer)
    {
        var payloadBuffer = buffer.GetBytes();
        var message = new byte[payloadBuffer.Length + 4];
        MapleAES.GetHeader(message, _sendVector, 76);
        Shanda.EncryptTransform(payloadBuffer);
        MapleAES.Transform(payloadBuffer, _sendVector);
        Buffer.BlockCopy(payloadBuffer, 0, message, 4, payloadBuffer.Length);
        for (var offset = 0; offset < message.Length;)
            offset += _socket.Send(message, offset, message.Length - offset, SocketFlags.None, out SocketError errorCode);
        // TODO errorCode
    }

    public void SendRaw(GameMessageBuffer buffer)
    {
        byte[] payloadBuffer = buffer.GetBytes();
        for (int offset = 0; offset < payloadBuffer.Length;)
            offset += _socket.Send(payloadBuffer, offset, payloadBuffer.Length - offset, SocketFlags.None, out SocketError errorCode);
        // TODO errorCode
    }

    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        await _socket.DisconnectAsync(false, cancellationToken);
        OnDisconnected?.Invoke(this);
    }
}