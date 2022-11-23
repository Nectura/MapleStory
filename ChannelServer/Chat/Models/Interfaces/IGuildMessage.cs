namespace ChannelServer.Chat.Models.Interfaces;

public interface IGuildMessage : IChatMessage
{
    uint GuildId { get; init; }
}