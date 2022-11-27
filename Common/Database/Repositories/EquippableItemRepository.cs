using Common.Database.Models;
using Common.Database.Repositories.Abstract;
using Common.Database.Repositories.Interfaces;

namespace Common.Database.Repositories;

public sealed class EquippableItemRepository : EntityRepository<EquippableItem>, IEquippableItemRepository
{
    public EquippableItemRepository(EntityContext context) : base(context)
    {
    }
}