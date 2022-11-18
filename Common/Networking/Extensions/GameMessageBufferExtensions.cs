using System.Reflection;
using Common.Networking.Packets.Attributes;

namespace Common.Networking.Extensions;

public static class GameMessageBufferExtensions
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