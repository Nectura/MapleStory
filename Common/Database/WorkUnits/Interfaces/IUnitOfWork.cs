namespace Common.Database.WorkUnits.Interfaces;

public interface IUnitOfWork
{
    Task<int> CommitChangesAsync(CancellationToken cancellationToken = default);
}