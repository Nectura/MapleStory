using Common.Database.Interfaces;
using Common.Database.Repositories;
using Common.Database.Repositories.Interfaces;
using Common.Database.WorkUnits.Abstract;
using Common.Database.WorkUnits.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Common.Database.WorkUnits;

public sealed class InventoryWorkUnit : UnitOfWork, IInventoryWorkUnit
{
    public InventoryWorkUnit(IEntityContext entityContext) : base((DbContext)entityContext)
    {
        Inventories = new InventoryRepository(entityContext);
        InventoryTabItems = new InventoryTabItemRepository(entityContext);
        Characters = new CharacterRepository(entityContext);
    }
    
    public ICharacterRepository Characters { get; }
    public IInventoryRepository Inventories { get; }
    public IInventoryTabItemRepository InventoryTabItems { get; }
}