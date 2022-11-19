using Common.Networking.Cryptography;
using Common.Networking.OperationCodes;
using System.Net.Sockets;
using static Common.Networking.Enums.EGameMessageType;

namespace Common.Networking;

public sealed class GameClient
{
    private readonly byte[] _buffer = new byte[4096];
    private int _size = 0;
    private readonly Socket _socket;
    private readonly MapleIV _sendVector, _recvVector;

    public event Action<GameClient, GameMessageBuffer>? OnMessage;

    public GameClient(Socket socket)
    {
        _socket = socket;
        _sendVector = new MapleIV((uint)Random.Shared.Next());
        _recvVector = new MapleIV((uint)Random.Shared.Next());
        SendRaw(new GameMessage(EServerOperationCode.Handshake)
        {
            { u16, (ushort)76 },
            { str, "1" },
            { u32, _recvVector.Value },
            { u32, _sendVector.Value },
            { u8, (byte)9 }
        });
        socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, out SocketError errorCode, OnReceive, null);
        // TODO errorCode
    }

    public void OnReceive(IAsyncResult ar)
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
        OnMessage?.Invoke(this, new GameMessageBuffer(payload));

        _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, out SocketError errorCode2, OnReceive, null);
    }

    public void SendRaw(GameMessage payload)
    {
        byte[] payloadBuffer = payload.GetMessageBuffer().GetBytes();
        //MapleAES.GetHeader(message, _sendVector, 76);
        //Shanda.EncryptTransform(payloadBuffer);
        //MapleAES.Transform(payloadBuffer, _sendVector);
        for (int offset = 0; offset < payloadBuffer.Length;)
            offset += _socket.Send(payloadBuffer, offset, payloadBuffer.Length - offset, SocketFlags.None, out SocketError errorCode);
        // TODO errorCode
    }

    public void Send(GameMessage payload)
    {
        var payloadBuffer = payload.GetMessageBuffer().GetBytes();
        var message = new byte[payloadBuffer.Length + 4];
        MapleAES.GetHeader(message, _sendVector, 76);
        Shanda.EncryptTransform(payloadBuffer);
        MapleAES.Transform(payloadBuffer, _sendVector);
        Buffer.BlockCopy(payloadBuffer, 0, message, 4, payloadBuffer.Length);
        for (var offset = 0; offset < message.Length;)
            offset += _socket.Send(message, offset, message.Length - offset, SocketFlags.None, out SocketError errorCode);
        // TODO errorCode
    }
}