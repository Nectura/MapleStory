using Common.Database.Repositories;
using Common.Database.Repositories.Interfaces;
using Common.Database.WorkUnits.Abstract;
using Common.Database.WorkUnits.Interfaces;

namespace Common.Database.WorkUnits;

public sealed class InventoryWorkUnit : UnitOfWork, IInventoryWorkUnit
{
    public InventoryWorkUnit(EntityContext entityContext) : base(entityContext)
    {
        Inventories = new InventoryRepository(entityContext);
        EquippableItems = new EquippableItemRepository(entityContext);
        ConsumableItems = new ConsumableItemRepository(entityContext);
        SetupItems = new SetupItemRepository(entityContext);
        EtceteraItems = new EtceteraItemRepository(entityContext);
    }
    
    public IInventoryRepository Inventories { get; }
    public IEquippableItemRepository EquippableItems { get; }
    public IConsumableItemRepository ConsumableItems { get; }
    public ISetupItemRepository SetupItems { get; }
    public IEtceteraItemRepository EtceteraItems { get; }
}