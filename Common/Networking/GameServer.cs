using System.Net;
using System.Net.Sockets;
using Common.Networking.OperationCodes;

namespace Common.Networking;

internal class GameServer
{
    public readonly HashSet<GameClient> ConnectedPeers = new();
    private readonly TcpListener _listener;

    public GameServer(IPEndPoint endpoint)
    {
        _listener = new TcpListener(endpoint);
        _listener.Start();
        Console.WriteLine("Started Listening For Connections.");
        _listener.BeginAcceptSocket(OnSocketAccepted, default);
    }

    private async void OnSocketAccepted(IAsyncResult state)
    {
        try
        {
            Socket socket = _listener.EndAcceptSocket(state);
            GameClient client = new(socket);
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

    private void OnClientMessage(GameMessageBuffer message)
    {
        EClientOperationCode op = (EClientOperationCode)message.Read16U();
        Console.WriteLine($"Client sent {nameof(op)} ({op:X}): " +
            $"{BitConverter.ToString(message.GetBytes()).Replace('-', ' ')}");
        //TODO handle message
    }
}