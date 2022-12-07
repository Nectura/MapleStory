using Common.Database.Repositories.Interfaces;

namespace Common.Database.WorkUnits.Interfaces;

public interface IInventoryWorkUnit : IUnitOfWork
{
    ICharacterRepository Characters { get; }
    IInventoryRepository Inventories { get; }
    IInventoryTabItemRepository InventoryTabItems { get; }
}