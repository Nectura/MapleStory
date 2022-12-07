namespace Common.Interfaces.Inventory;

public interface IItem
{
    Guid Id { get; set; }
    uint MapleId { get; set; }
    bool IsNxItem { get; set; }
    string? NameTag { get; set; }
}