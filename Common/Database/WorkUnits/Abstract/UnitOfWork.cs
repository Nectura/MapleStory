namespace Common.Database.WorkUnits.Abstract;

public abstract class UnitOfWork
{
    protected readonly EntityContext _entityContext;

    protected UnitOfWork(EntityContext entityContext)
    {
        _entityContext = entityContext;
    }

    public async ValueTask DisposeAsync()
    {
        await _entityContext.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    public async Task<int> CommitChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _entityContext.SaveChangesAsync(cancellationToken);
    }
}