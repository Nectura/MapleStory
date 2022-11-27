using Common.Database.Repositories.Interfaces;

namespace Common.Database.WorkUnits.Interfaces;

public interface IInventoryWorkUnit : IUnitOfWork
{
    IInventoryRepository Inventories { get; }
    IEquippableItemRepository EquippableItems { get; }
    IConsumableItemRepository ConsumableItems { get; }
    ISetupItemRepository SetupItems { get; }
    IEtceteraItemRepository EtceteraItems { get; }
}