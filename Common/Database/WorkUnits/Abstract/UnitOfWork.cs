using Microsoft.EntityFrameworkCore;

namespace Common.Database.WorkUnits.Abstract;

public abstract class UnitOfWork
{
    protected readonly DbContext _dbContext;

    protected UnitOfWork(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    public async Task<int> CommitChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}