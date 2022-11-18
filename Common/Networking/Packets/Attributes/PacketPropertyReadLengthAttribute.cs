namespace Common.Networking.Packets.Attributes;

public class PacketPropertyReadLengthAttribute : Attribute
{
    public int Length { get; set; }

    public PacketPropertyReadLengthAttribute(int length)
    {
        Length = length;
    }
}