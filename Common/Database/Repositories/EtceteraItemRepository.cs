using Common.Database.Models;
using Common.Database.Repositories.Abstract;
using Common.Database.Repositories.Interfaces;

namespace Common.Database.Repositories;

public sealed class EtceteraItemRepository : EntityRepository<EtceteraItem>, IEtceteraItemRepository
{
    public EtceteraItemRepository(EntityContext context) : base(context)
    {
    }
}