using System.Net.Sockets;

namespace Common.Networking.Models;

public sealed class TcpGameClient : SocketClient
{
    public TcpGameClient(Socket socket) : base(socket) {}
}