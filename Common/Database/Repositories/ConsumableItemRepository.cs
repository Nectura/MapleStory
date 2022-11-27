using Common.Database.Models;
using Common.Database.Repositories.Abstract;
using Common.Database.Repositories.Interfaces;

namespace Common.Database.Repositories;

public sealed class ConsumableItemRepository : EntityRepository<ConsumableItem>, IConsumableItemRepository
{
    public ConsumableItemRepository(EntityContext context) : base(context)
    {
    }
}