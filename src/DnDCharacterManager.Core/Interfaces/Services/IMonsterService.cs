using DnDCharacterManager.Core.DTOs;
using DnDCharacterManager.Core.Enums;

namespace DnDCharacterManager.Core.Interfaces.Services;

public interface IMonsterService
{
    Task<IReadOnlyList<MonsterDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<MonsterDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<MonsterDto> CreateAsync(MonsterDto dto, CancellationToken cancellationToken = default);

    Task<MonsterDto?> UpdateAsync(Guid id, MonsterDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MonsterDto>> SearchAsync(string name, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MonsterDto>> GetByCreatureTypeAsync(CreatureType creatureType, CancellationToken cancellationToken = default);
}
