namespace Common.Interfaces.Inventory;

public interface IItem
{
    public Guid Id { get; init; }
    public uint MapleId { get; init; }
}