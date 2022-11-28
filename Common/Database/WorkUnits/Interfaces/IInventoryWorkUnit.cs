using Common.Database.Repositories.Interfaces;

namespace Common.Database.WorkUnits.Interfaces;

public interface IInventoryWorkUnit : IUnitOfWork
{
    IInventoryRepository Inventories { get; }
    IInventoryTabItemRepository InventoryTabItems { get; }
}