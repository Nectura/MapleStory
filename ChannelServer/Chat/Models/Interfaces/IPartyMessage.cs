namespace ChannelServer.Chat.Models.Interfaces;

public interface IPartyMessage : IChatMessage
{
    uint PartyId { get; init; }
}