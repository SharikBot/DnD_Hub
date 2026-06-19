using DnDCharacterManager.Core.Entities;
using DnDCharacterManager.Core.Interfaces.Repositories;
using DnDCharacterManager.Core.Interfaces.Services;

namespace DnDCharacterManager.Application.Services;

public class ReferenceService : IReferenceService
{
    private readonly IReferenceRepository _referenceRepository;

    public ReferenceService(IReferenceRepository referenceRepository)
    {
        _referenceRepository = referenceRepository;
    }

    public Task<IReadOnlyList<Race>> GetRacesAsync(CancellationToken cancellationToken = default) =>
        _referenceRepository.GetRacesAsync(cancellationToken);

    public Task<IReadOnlyList<CharacterClass>> GetClassesAsync(CancellationToken cancellationToken = default) =>
        _referenceRepository.GetClassesAsync(cancellationToken);

    public Task<IReadOnlyList<Background>> GetBackgroundsAsync(CancellationToken cancellationToken = default) =>
        _referenceRepository.GetBackgroundsAsync(cancellationToken);

    public Task<IReadOnlyList<Trait>> GetTraitsAsync(CancellationToken cancellationToken = default) =>
        _referenceRepository.GetTraitsAsync(cancellationToken);

    public Task<IReadOnlyList<Spell>> GetSpellsAsync(CancellationToken cancellationToken = default) =>
        _referenceRepository.GetSpellsAsync(cancellationToken);
}
