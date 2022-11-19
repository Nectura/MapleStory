namespace Common.Networking.Packets.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class PacketPropertyAttribute : Attribute
{
    private const int ReadLengthDefaultValue = -1;
    
    public uint Order { get; set; }
    public int ReadLength { get; set; }
    
    public bool HasReadLength => ReadLength != ReadLengthDefaultValue;

    public PacketPropertyAttribute(uint order, int readLength = ReadLengthDefaultValue)
    {
        Order = order;
        ReadLength = readLength;
    }
}