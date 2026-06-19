namespace DnDCharacterManager.Core.Interfaces.Repositories;

public interface IUnitOfWork : IAsyncDisposable
{
    ICharacterRepository Characters { get; }

    IMonsterRepository Monsters { get; }

    IRuleRepository Rules { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
