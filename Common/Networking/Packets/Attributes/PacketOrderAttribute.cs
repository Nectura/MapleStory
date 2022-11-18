namespace Common.Networking.Packets.Attributes;

public class PacketOrderAttribute : Attribute
{
    public uint Order { get; set; }

    public PacketOrderAttribute(uint orderNum)
    {
        Order = orderNum;
    }
}