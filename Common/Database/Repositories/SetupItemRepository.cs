using Common.Database.Models;
using Common.Database.Repositories.Abstract;
using Common.Database.Repositories.Interfaces;

namespace Common.Database.Repositories;

public sealed class SetupItemRepository : EntityRepository<SetupItem>, ISetupItemRepository
{
    public SetupItemRepository(EntityContext context) : base(context)
    {
    }
}