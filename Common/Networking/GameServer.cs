using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Common.Networking.Enums;
using Common.Networking.OperationCodes;
using Newtonsoft.Json;

namespace Common.Networking;

public sealed class GameServer
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

    private void OnClientMessage(GameClient client, GameMessageBuffer message)
    {
        var op = (EClientOperationCode)message.Read16U();
        
        Console.WriteLine($"Client sent {Enum.GetName(typeof(EClientOperationCode), op)} ({op:X}): {BitConverter.ToString(message.GetBytes()).Replace('-', ' ')}");
        
        if (op is EClientOperationCode.ClientStart)
        {
            client.Send(new GameMessage(EServerOperationCode.SetLoginBg)
            {
                { EGameMessageType.str, "MapLogin" }
            });
        }

        if (op is EClientOperationCode.ClientLogin)
        { 
            Test.Read<ClientLoginPacket>(new GameMessageBuffer(message.GetBytes()), out var packetInstance); 
            Console.WriteLine($"Received And Serialized Packet Instance: {JsonConvert.SerializeObject(packetInstance)}");
        }
    }
}

[PacketHandler(1)]
public record ClientLoginPacket
{ 
    [PacketOrder(0)] // Temporarily here only because of Reader logic malfunction apparently, the opcode should not be inside those classes.
    public short Opcode { get; init; }
    
    [PacketOrder(1)]
    public string UserName { get; init; }
    
    [PacketOrder(2)]
    public string Password { get; init; }
}

public interface PacketHandler
{
    // something here

}

public static class Test
{
    private struct PacketProperty
    {
        public PropertyInfo PropertyInfo { get; init; }
        public int? Length { get; init; }
    }
    
    public static void Read<T>(GameMessageBuffer buffer, out T packetInstance) where T : class
    {
        packetInstance = Activator.CreateInstance<T>();
        var dict = new SortedDictionary<uint, PacketProperty>();
        
        // get the order of each property and append them in a sorted manner to a dictionary
        foreach (var property in packetInstance.GetType().GetProperties())
        {
            var packetOrderAttr = property.GetCustomAttribute<PacketOrderAttribute>();
            var packetReaderLenAttr = property.GetCustomAttribute<PacketPropertyReadLengthAttribute>();
            dict.Add(packetOrderAttr!.Order, new PacketProperty
            {
                PropertyInfo = property,
                Length = packetReaderLenAttr?.Length
            });
        }

        // iterate over the properties in the dictionary and try to read them with our GameMessageBuffer
        foreach (var packetProperty in dict.Values)
        {
            if (!buffer.TryRead(packetProperty.PropertyInfo.PropertyType, packetProperty.Length, out var val)) continue;
            packetProperty.PropertyInfo.SetValue(packetInstance, val);
        }
    } 
}

public class PacketHandlerAttribute : Attribute
{
    public ushort OperationCode { get; set; }

    public PacketHandlerAttribute(ushort opcode)
    {
        OperationCode = opcode;
    }
}
public class PacketOrderAttribute : Attribute
{
    public uint Order { get; set; }

    public PacketOrderAttribute(uint orderNum)
    {
        Order = orderNum;
    }
}

public class PacketPropertyReadLengthAttribute : Attribute
{
    public int Length { get; set; }

    public PacketPropertyReadLengthAttribute(int length)
    {
        Length = length;
    }
}


