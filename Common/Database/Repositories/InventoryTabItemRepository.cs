using Common.Database.Interfaces;
using Common.Database.Models;
using Common.Database.Repositories.Abstract;
using Common.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Common.Database.Repositories;

public sealed class InventoryTabItemRepository : EntityRepository<InventoryTabItem>, IInventoryTabItemRepository
{
    public InventoryTabItemRepository(IEntityContext context) : base((DbContext)context)
    {
    }
}