using System.Net;
using System.Net.Sockets;

namespace Common.Networking.Extensions;

public static class SocketExtensions
{
    public static IPAddress GetRemoteIpAddress(this Socket socket)
    {        
        if (socket.RemoteEndPoint == default)
            throw new NullReferenceException();
        
        return ((IPEndPoint)socket.RemoteEndPoint).Address;
    }
}