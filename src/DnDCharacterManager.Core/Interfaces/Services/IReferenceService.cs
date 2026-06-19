using DnDCharacterManager.Core.Entities;

namespace DnDCharacterManager.Core.Interfaces.Services;

public interface IReferenceService
{
    Task<IReadOnlyList<Race>> GetRacesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CharacterClass>> GetClassesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Background>> GetBackgroundsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Trait>> GetTraitsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Spell>> GetSpellsAsync(CancellationToken cancellationToken = default);
}
