using System.Reflection;
using Common.Networking;

namespace LoginServer.Handlers;

[PacketHandler(1)]
public record struct ClientLoginPacket
{
    [PacketOrder(0)]
    public string UserName { get; init; }
    
    [PacketOrder(1)]
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


