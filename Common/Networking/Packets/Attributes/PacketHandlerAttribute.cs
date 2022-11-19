using Common.Networking.Packets.Enums;

namespace Common.Networking.Packets.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class PacketHandlerAttribute : Attribute
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