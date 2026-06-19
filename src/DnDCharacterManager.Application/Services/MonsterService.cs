using DnDCharacterManager.Core.DTOs;
using DnDCharacterManager.Core.Entities;
using DnDCharacterManager.Core.Enums;
using DnDCharacterManager.Core.Interfaces.Repositories;
using DnDCharacterManager.Core.Interfaces.Services;

namespace DnDCharacterManager.Application.Services;

public class MonsterService : IMonsterService
{
    private readonly IUnitOfWork _unitOfWork;

    public MonsterService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<MonsterDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var monsters = await _unitOfWork.Monsters.GetAllAsync(cancellationToken);
        return monsters.Select(MapToDto).ToList();
    }

    public async Task<MonsterDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var monster = await _unitOfWork.Monsters.GetByIdAsync(id, cancellationToken);
        return monster is null ? null : MapToDto(monster);
    }

    public async Task<MonsterDto> CreateAsync(MonsterDto dto, CancellationToken cancellationToken = default)
    {
        var entity = MapToEntity(dto);
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Monsters.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
    }

    public async Task<MonsterDto?> UpdateAsync(Guid id, MonsterDto dto, CancellationToken cancellationToken = default)
    {
        var monster = await _unitOfWork.Monsters.GetByIdAsync(id, cancellationToken);
        if (monster is null)
        {
            return null;
        }

        monster.Name = dto.Name.Trim();
        monster.ChallengeRating = dto.ChallengeRating;
        monster.CreatureType = dto.CreatureType;
        monster.ArmorClass = dto.ArmorClass;
        monster.HitPoints = dto.HitPoints;
        monster.Strength = GetScore(dto, AbilityType.Str);
        monster.Dexterity = GetScore(dto, AbilityType.Dex);
        monster.Constitution = GetScore(dto, AbilityType.Con);
        monster.Intelligence = GetScore(dto, AbilityType.Int);
        monster.Wisdom = GetScore(dto, AbilityType.Wis);
        monster.Charisma = GetScore(dto, AbilityType.Cha);
        monster.Description = dto.Description;
        monster.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Monsters.UpdateAsync(monster, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(monster);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Monsters.ExistsAsync(id, cancellationToken))
        {
            return false;
        }

        await _unitOfWork.Monsters.DeleteAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<MonsterDto>> SearchAsync(string name, CancellationToken cancellationToken = default)
    {
        var monsters = await _unitOfWork.Monsters.SearchByNameAsync(name, cancellationToken);
        return monsters.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<MonsterDto>> GetByCreatureTypeAsync(CreatureType creatureType, CancellationToken cancellationToken = default)
    {
        var monsters = await _unitOfWork.Monsters.GetByCreatureTypeAsync(creatureType, cancellationToken);
        return monsters.Select(MapToDto).ToList();
    }

    private static MonsterDto MapToDto(Monster monster) => new()
    {
        Id = monster.Id,
        Name = monster.Name,
        ChallengeRating = monster.ChallengeRating,
        CreatureType = monster.CreatureType,
        ArmorClass = monster.ArmorClass,
        HitPoints = monster.HitPoints,
        AbilityScores = new Dictionary<AbilityType, int>
        {
            [AbilityType.Str] = monster.Strength,
            [AbilityType.Dex] = monster.Dexterity,
            [AbilityType.Con] = monster.Constitution,
            [AbilityType.Int] = monster.Intelligence,
            [AbilityType.Wis] = monster.Wisdom,
            [AbilityType.Cha] = monster.Charisma
        },
        Description = monster.Description
    };

    private static Monster MapToEntity(MonsterDto dto) => new()
    {
        Name = dto.Name.Trim(),
        ChallengeRating = dto.ChallengeRating,
        CreatureType = dto.CreatureType,
        ArmorClass = dto.ArmorClass,
        HitPoints = dto.HitPoints,
        Speed = 30,
        Strength = GetScore(dto, AbilityType.Str),
        Dexterity = GetScore(dto, AbilityType.Dex),
        Constitution = GetScore(dto, AbilityType.Con),
        Intelligence = GetScore(dto, AbilityType.Int),
        Wisdom = GetScore(dto, AbilityType.Wis),
        Charisma = GetScore(dto, AbilityType.Cha),
        Description = dto.Description
    };

    private static int GetScore(MonsterDto dto, AbilityType ability) =>
        dto.AbilityScores.TryGetValue(ability, out var value) ? value : 10;
}
