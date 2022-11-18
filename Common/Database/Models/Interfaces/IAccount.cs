using Common.Enums;

namespace Common.Database.Models.Interfaces;

public interface IAccount
{
    int Id { get; set; }
    string UserName { get; set; }
    byte[] PasswordHash { get; set; }
    byte[] PasswordSaltHash { get; set; }
    byte[] PinHash { get; set; }
    byte[] PinSaltHash { get; set; }
    byte[] PicHash { get; set; }
    byte[] PicSaltHash { get; set; }
    int CharacterSlots { get; set; }
    EWorld? LastWorldId { get; set; }
    string? LastKnownIpAddress { get; set; }
    EAccountType AccountType { get; set; }
    DateTime CreatedAt { get; set; }
    Guid? RestrictionId { get; set; }
    bool IsAdmin { get; }
    bool IsRestricted { get; }
    bool HasPin { get; }
    bool HasPic { get; }
}