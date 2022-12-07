using Common.Database.Enums;
using Common.Enums;

namespace Common.Database.Models.Interfaces;

public interface IAccount
{
    uint Id { get; set; }
    string UserName { get; set; }
    byte[] PasswordHash { get; set; }
    byte[] PasswordSaltHash { get; set; }
    byte[] PinHash { get; set; }
    byte[] PinSaltHash { get; set; }
    byte[] PicHash { get; set; }
    byte[] PicSaltHash { get; set; }
    bool HasAcceptedEula { get; set; }
    uint CharacterSlots { get; set; }
    EWorld? LastWorldId { get; set; }
    string? LastKnownIpAddress { get; set; }
    EAccountType AccountType { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime LastLoggedInAt { get; set; }
    Guid? RestrictionId { get; set; }
    bool IsAdmin { get; }
    bool IsRestricted { get; }
    bool HasPin { get; }
    bool HasPic { get; }
}