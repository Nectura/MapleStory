using Common.InventoryX.Enums;

namespace Common.InventoryX.Interfaces;

public interface IItem
{
    public Guid Id { get; init; }
    public uint MapleId { get; init; }
}