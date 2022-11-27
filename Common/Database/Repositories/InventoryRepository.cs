using Common.Database.Repositories.Abstract;
using Common.Database.Repositories.Interfaces;

namespace Common.Database.Repositories;

public sealed class InventoryRepository : EntityRepository<Models.Inventory>, IInventoryRepository
{
    public InventoryRepository(EntityContext context) : base(context)
    {
    }
}