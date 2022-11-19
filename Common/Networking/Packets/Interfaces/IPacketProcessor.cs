namespace Common.Networking.Packets.Interfaces;

public interface IPacketProcessor
{
    void ProcessPacket(GameClient client, GameMessageBuffer buffer);
}