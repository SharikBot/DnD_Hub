using DnDCharacterManager.Core.Entities;

namespace DnDCharacterManager.Core.Interfaces.Repositories;

public interface ICharacterRepository : IRepository<Character>
{
    Task<IReadOnlyList<Character>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Character?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}
