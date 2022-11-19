using System.ComponentModel.DataAnnotations.Schema;
using Common.Database.Enums;
using Common.Database.Models.Interfaces;
using Common.Enums;

namespace Common.Database.Models;

public class Account : IAccount
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
    public byte[] PasswordSaltHash { get; set; } = Array.Empty<byte>();
    public byte[] PinHash { get; set; } = Array.Empty<byte>();
    public byte[] PinSaltHash { get; set; } = Array.Empty<byte>();
    public byte[] PicHash { get; set; } = Array.Empty<byte>();
    public byte[] PicSaltHash { get; set; } = Array.Empty<byte>();
    public bool HasAcceptedEula { get; set; } = false;
    public int CharacterSlots { get; set; } = 5;
    public EWorld? LastWorldId { get; set; }
    public string? LastKnownIpAddress { get; set; }
    public EAccountType AccountType { get; set; } = EAccountType.Normal;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastLoggedInAt { get; set; }

    [ForeignKey(nameof(Restriction))]
    public Guid? RestrictionId { get; set; }
    public virtual AccountRestriction? Restriction { get; set; }

    [NotMapped] public bool IsAdmin => AccountType == EAccountType.Normal;

    [NotMapped] public bool IsRestricted => RestrictionId.HasValue;

    [NotMapped] public bool HasPin => PinHash != Array.Empty<byte>();

    [NotMapped] public bool HasPic => PicHash != Array.Empty<byte>();

    public virtual ICollection<Character>? Characters { get; set; }
}