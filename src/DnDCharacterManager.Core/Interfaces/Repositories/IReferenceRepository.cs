using DnDCharacterManager.Core.Entities;

namespace DnDCharacterManager.Core.Interfaces.Repositories;

public interface IReferenceRepository
{
    Task<IReadOnlyList<Race>> GetRacesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CharacterClass>> GetClassesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Background>> GetBackgroundsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Trait>> GetTraitsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Spell>> GetSpellsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Skill>> GetSkillsAsync(CancellationToken cancellationToken = default);

    Task<Race?> GetRaceByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CharacterClass?> GetClassByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Background?> GetBackgroundByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
