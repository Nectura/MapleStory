namespace Common.Networking.Packets.Attributes;

public class PacketHandlerAttribute : Attribute
{
    public ushort OperationCode { get; set; }

    public PacketHandlerAttribute(ushort opcode)
    {
        OperationCode = opcode;
    }
}