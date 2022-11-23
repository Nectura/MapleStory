namespace ChannelServer.Chat.Models.Interfaces;

public interface IGeneralMessage : IChatMessage
{
    uint MapId { get; init; }
}