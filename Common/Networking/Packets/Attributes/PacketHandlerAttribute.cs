using Common.Networking.OperationCodes;

namespace Common.Networking.Packets.Attributes;

public class PacketHandlerAttribute : Attribute
{
    public EClientOperationCode OperationCode { get; set; }

    public PacketHandlerAttribute(EClientOperationCode opcode)
    {
        OperationCode = opcode;
    }

    public PacketHandlerAttribute(ushort opcode)
    {
        if (!Enum.IsDefined(typeof(EClientOperationCode), opcode))
            throw new ArgumentException($"The opcode specified '{opcode}' does not match a known value.");
        OperationCode = (EClientOperationCode)opcode;
    }
}