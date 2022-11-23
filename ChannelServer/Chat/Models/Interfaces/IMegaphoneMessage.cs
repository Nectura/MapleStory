namespace ChannelServer.Chat.Models.Interfaces;

public interface IMegaphoneMessage : IChatMessage
{
    uint ChannelId { get; init; }
}