using Common.Database.Models;
using Common.Database.Repositories.Abstract.Interfaces;

namespace Common.Database.Repositories.Interfaces;

public interface IInventoryRepository : IEntityRepository<Inventory>
{
}