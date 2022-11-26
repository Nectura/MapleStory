namespace Common.Networking.Packets.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public sealed class PacketFieldAttribute : Attribute
{
    private const int ReadLengthDefaultValue = -1;
    
    public uint Order { get; set; }
    public int ReadLength { get; set; }
    
    public bool HasReadLength => ReadLength != ReadLengthDefaultValue;

    public PacketFieldAttribute(uint order, int readLength = ReadLengthDefaultValue)
    {
        Order = order;
        ReadLength = readLength;
    }
}