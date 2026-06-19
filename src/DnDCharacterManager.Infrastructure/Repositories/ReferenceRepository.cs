using DnDCharacterManager.Core.Entities;
using DnDCharacterManager.Core.Interfaces.Repositories;
using DnDCharacterManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DnDCharacterManager.Infrastructure.Repositories;

public class ReferenceRepository : IReferenceRepository
{
    private readonly AppDbContext _context;

    public ReferenceRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<Race>> GetRacesAsync(CancellationToken cancellationToken = default) =>
        await _context.Races.AsNoTracking().OrderBy(r => r.Name).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<CharacterClass>> GetClassesAsync(CancellationToken cancellationToken = default) =>
        await _context.CharacterClasses.AsNoTracking().OrderBy(c => c.Name).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Background>> GetBackgroundsAsync(CancellationToken cancellationToken = default) =>
        await _context.Backgrounds.AsNoTracking().OrderBy(b => b.Name).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Trait>> GetTraitsAsync(CancellationToken cancellationToken = default) =>
        await _context.Traits.AsNoTracking().OrderBy(t => t.Name).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Spell>> GetSpellsAsync(CancellationToken cancellationToken = default) =>
        await _context.Spells.AsNoTracking().OrderBy(s => s.Level).ThenBy(s => s.Name).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Skill>> GetSkillsAsync(CancellationToken cancellationToken = default) =>
        await _context.Skills.AsNoTracking().OrderBy(s => s.Name).ToListAsync(cancellationToken);

    public Task<Race?> GetRaceByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Races.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public Task<CharacterClass?> GetClassByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.CharacterClasses.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<Background?> GetBackgroundByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Backgrounds.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
}
