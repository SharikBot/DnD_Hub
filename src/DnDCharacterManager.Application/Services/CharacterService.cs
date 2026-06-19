using DnDCharacterManager.Core.DTOs;
using DnDCharacterManager.Core.Entities;
using DnDCharacterManager.Core.Enums;
using DnDCharacterManager.Core.Interfaces.Repositories;
using DnDCharacterManager.Core.Interfaces.Services;
using DnDCharacterManager.Core.Patterns.Factory;
using DnDCharacterManager.Core.Patterns.Strategy;
using DnDCharacterManager.Application.Validators;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace DnDCharacterManager.Application.Services;

public class CharacterService : ICharacterService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    private readonly IUnitOfWork _unitOfWork;
    private readonly IReferenceRepository _referenceRepository;
    private readonly ICharacterFactory _characterFactory;
    private readonly IMemoryCache _cache;
    private readonly CreateCharacterValidator _validator;
    private readonly ILogger<CharacterService> _logger;

    public CharacterService(
        IUnitOfWork unitOfWork,
        IReferenceRepository referenceRepository,
        ICharacterFactory characterFactory,
        IMemoryCache cache,
        CreateCharacterValidator validator,
        ILogger<CharacterService> logger)
    {
        _unitOfWork = unitOfWork;
        _referenceRepository = referenceRepository;
        _characterFactory = characterFactory;
        _cache = cache;
        _validator = validator;
        _logger = logger;
    }

    public async Task<CharacterDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey(id);
        if (_cache.TryGetValue(cacheKey, out CharacterDto? cached))
        {
            return cached;
        }

        var character = await _unitOfWork.Characters.GetByIdWithDetailsAsync(id, cancellationToken);
        if (character is null)
        {
            return null;
        }

        var dto = MapToDto(character);
        _cache.Set(cacheKey, dto, CacheDuration);
        return dto;
    }

    public async Task<IReadOnlyList<CharacterListItemDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var characters = await _unitOfWork.Characters.GetByUserIdAsync(userId, cancellationToken);
        return characters.Select(MapToListItem).ToList();
    }

    public async Task<CharacterDto> CreateAsync(CreateCharacterDto dto, CancellationToken cancellationToken = default)
    {
        var errors = _validator.Validate(dto);
        if (errors.Count > 0)
        {
            throw new ArgumentException(string.Join(" ", errors));
        }

        var abilityScores = ResolveAbilityScores(dto);
        var race = await _referenceRepository.GetRaceByIdAsync(dto.RaceId, cancellationToken)
            ?? throw new ArgumentException("Race not found.");
        var characterClass = await _referenceRepository.GetClassByIdAsync(dto.CharacterClassId, cancellationToken)
            ?? throw new ArgumentException("Class not found.");
        var background = await _referenceRepository.GetBackgroundByIdAsync(dto.BackgroundId, cancellationToken)
            ?? throw new ArgumentException("Background not found.");

        ApplyRaceBonuses(abilityScores, race);

        var character = _characterFactory.Create(dto, abilityScores);
        character.PortraitPath = dto.PortraitPath?.Trim();
        character.Speed = race.BaseSpeed;
        ApplyCombatStats(character, characterClass);

        await ApplyTraitsAsync(character, dto.TraitIds, cancellationToken);
        await ApplySpellsAsync(character, dto.SpellIds, cancellationToken);
        await ApplySkillsAsync(character, background, abilityScores, cancellationToken);
        ApplyEquipment(character, dto.EquipmentItems);

        await _unitOfWork.Characters.AddAsync(character, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _unitOfWork.Characters.GetByIdWithDetailsAsync(character.Id, cancellationToken)
            ?? character;

        _logger.LogInformation("Created character {CharacterId} for user {UserId}", created.Id, created.UserId);
        return MapToDto(created);
    }

    public async Task<CharacterDto?> UpdateAsync(Guid id, CreateCharacterDto dto, CancellationToken cancellationToken = default)
    {
        var errors = _validator.Validate(dto);
        if (errors.Count > 0)
        {
            throw new ArgumentException(string.Join(" ", errors));
        }

        var character = await _unitOfWork.Characters.GetByIdWithDetailsAsync(id, cancellationToken);
        if (character is null)
        {
            return null;
        }

        var abilityScores = ResolveAbilityScores(dto);
        var race = await _referenceRepository.GetRaceByIdAsync(dto.RaceId, cancellationToken);
        if (race is not null)
        {
            ApplyRaceBonuses(abilityScores, race);
            character.Speed = race.BaseSpeed;
        }

        var characterClass = await _referenceRepository.GetClassByIdAsync(dto.CharacterClassId, cancellationToken);

        character.Name = dto.Name.Trim();
        character.UserId = dto.UserId;
        character.RaceId = dto.RaceId;
        character.CharacterClassId = dto.CharacterClassId;
        character.BackgroundId = dto.BackgroundId;
        character.Alignment = dto.Alignment;
        character.Backstory = dto.Backstory?.Trim();
        character.PortraitPath = dto.PortraitPath?.Trim();
        character.Strength = GetScore(abilityScores, AbilityType.Str);
        character.Dexterity = GetScore(abilityScores, AbilityType.Dex);
        character.Constitution = GetScore(abilityScores, AbilityType.Con);
        character.Intelligence = GetScore(abilityScores, AbilityType.Int);
        character.Wisdom = GetScore(abilityScores, AbilityType.Wis);
        character.Charisma = GetScore(abilityScores, AbilityType.Cha);
        character.UpdatedAt = DateTime.UtcNow;

        if (characterClass is not null)
        {
            ApplyCombatStats(character, characterClass);
        }

        await _unitOfWork.Characters.UpdateAsync(character, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _cache.Remove(GetCacheKey(id));

        var updated = await _unitOfWork.Characters.GetByIdWithDetailsAsync(id, cancellationToken) ?? character;
        return MapToDto(updated);
    }

    public async Task<CharacterDto?> UpdateSheetAsync(Guid id, UpdateCharacterSheetDto dto, CancellationToken cancellationToken = default)
    {
        var character = await _unitOfWork.Characters.GetByIdWithDetailsAsync(id, cancellationToken);
        if (character is null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(dto.Name))
        {
            character.Name = dto.Name.Trim();
        }

        if (dto.CurrentHitPoints.HasValue)
        {
            character.CurrentHitPoints = Math.Clamp(dto.CurrentHitPoints.Value, 0, character.MaxHitPoints);
        }

        if (dto.Backstory is not null)
        {
            character.Backstory = dto.Backstory.Trim();
        }

        character.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.Characters.UpdateAsync(character, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _cache.Remove(GetCacheKey(id));

        return MapToDto(character);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Characters.ExistsAsync(id, cancellationToken))
        {
            return false;
        }

        await _unitOfWork.Characters.DeleteAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _cache.Remove(GetCacheKey(id));
        return true;
    }

    private static void ApplyRaceBonuses(Dictionary<AbilityType, int> scores, Race race)
    {
        scores[AbilityType.Str] = GetScore(scores, AbilityType.Str) + race.StrengthBonus;
        scores[AbilityType.Dex] = GetScore(scores, AbilityType.Dex) + race.DexterityBonus;
        scores[AbilityType.Con] = GetScore(scores, AbilityType.Con) + race.ConstitutionBonus;
        scores[AbilityType.Int] = GetScore(scores, AbilityType.Int) + race.IntelligenceBonus;
        scores[AbilityType.Wis] = GetScore(scores, AbilityType.Wis) + race.WisdomBonus;
        scores[AbilityType.Cha] = GetScore(scores, AbilityType.Cha) + race.CharismaBonus;
    }

    private static void ApplyCombatStats(Character character, CharacterClass characterClass)
    {
        var conMod = Modifier(character.Constitution);
        var hitDie = ParseHitDie(characterClass.HitDie);
        character.MaxHitPoints = Math.Max(1, hitDie + conMod);
        character.CurrentHitPoints = character.MaxHitPoints;
        character.ArmorClass = characterClass.BaseArmorClass > 0
            ? characterClass.BaseArmorClass + Modifier(character.Dexterity)
            : 10 + Modifier(character.Dexterity);
    }

    private async Task ApplyTraitsAsync(Character character, IReadOnlyList<Guid> traitIds, CancellationToken cancellationToken)
    {
        foreach (var traitId in traitIds.Distinct())
        {
            character.CharacterTraits.Add(new CharacterTrait
            {
                CharacterId = character.Id,
                TraitId = traitId,
            });
        }

        await Task.CompletedTask;
    }

    private async Task ApplySpellsAsync(Character character, IReadOnlyList<Guid> spellIds, CancellationToken cancellationToken)
    {
        foreach (var spellId in spellIds.Distinct())
        {
            character.CharacterSpells.Add(new CharacterSpell
            {
                CharacterId = character.Id,
                SpellId = spellId,
                IsPrepared = true,
                IsCantrip = false,
            });
        }

        await Task.CompletedTask;
    }

    private async Task ApplySkillsAsync(
        Character character,
        Background background,
        Dictionary<AbilityType, int> scores,
        CancellationToken cancellationToken)
    {
        var allSkills = await _referenceRepository.GetSkillsAsync(cancellationToken);
        var proficient = background.SkillProficiencies
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(s => s.ToLowerInvariant())
            .ToHashSet();

        foreach (var skill in allSkills)
        {
            var isProficient = proficient.Contains(skill.Name.ToLowerInvariant());
            character.CharacterSkills.Add(new CharacterSkill
            {
                CharacterId = character.Id,
                SkillId = skill.Id,
                IsProficient = isProficient,
            });
        }
    }

    private static void ApplyEquipment(Character character, IReadOnlyList<string> items)
    {
        if (character.Inventory is null)
        {
            return;
        }

        foreach (var itemName in items.Where(i => !string.IsNullOrWhiteSpace(i)).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            character.Inventory.Items.Add(new InventoryItem
            {
                Id = Guid.NewGuid(),
                InventoryId = character.Inventory.Id,
                Name = itemName.Trim(),
                Quantity = 1,
                Weight = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            });
        }
    }

    private static int ParseHitDie(string hitDie)
    {
        var digits = new string(hitDie.Where(char.IsDigit).ToArray());
        return int.TryParse(digits, out var value) ? value : 8;
    }

    private static Dictionary<AbilityType, int> ResolveAbilityScores(CreateCharacterDto dto)
    {
        IAbilityScoreStrategy strategy = dto.AbilityScoreMethod switch
        {
            AbilityScoreMethod.Roll4D6 => new Roll4D6Strategy(),
            AbilityScoreMethod.PointBuy => new PointBuyStrategy(),
            _ => new StandardArrayStrategy()
        };

        return strategy.GenerateScores(dto.AbilityScores);
    }

    private static int GetScore(Dictionary<AbilityType, int> scores, AbilityType ability) =>
        scores.TryGetValue(ability, out var value) ? value : 10;

    private static int Modifier(int score) => (score - 10) / 2;

    private static int ProficiencyBonus(int level) => 2 + (level - 1) / 4;

    private static string GetCacheKey(Guid id) => $"character:{id}";

    private static CharacterDto MapToDto(Character character)
    {
        var prof = ProficiencyBonus(character.Level);
        var scores = new Dictionary<AbilityType, int>
        {
            [AbilityType.Str] = character.Strength,
            [AbilityType.Dex] = character.Dexterity,
            [AbilityType.Con] = character.Constitution,
            [AbilityType.Int] = character.Intelligence,
            [AbilityType.Wis] = character.Wisdom,
            [AbilityType.Cha] = character.Charisma,
        };

        return new CharacterDto
        {
            Id = character.Id,
            Name = character.Name,
            Level = character.Level,
            Alignment = character.Alignment,
            Backstory = character.Backstory,
            PortraitPath = character.PortraitPath,
            AbilityScores = scores,
            CurrentHitPoints = character.CurrentHitPoints,
            MaxHitPoints = character.MaxHitPoints,
            ArmorClass = character.ArmorClass,
            Speed = character.Speed,
            InitiativeBonus = Modifier(character.Dexterity),
            ProficiencyBonus = prof,
            RaceName = character.Race?.Name ?? string.Empty,
            ClassName = character.CharacterClass?.Name ?? string.Empty,
            BackgroundName = character.Background?.Name ?? string.Empty,
            UserId = character.UserId,
            CreatedAt = character.CreatedAt,
            Traits = character.CharacterTraits.Select(ct => new CharacterTraitItemDto
            {
                TraitId = ct.TraitId,
                Name = ct.Trait?.Name ?? string.Empty,
                Description = ct.Trait?.Description ?? string.Empty,
            }).ToList(),
            Spells = character.CharacterSpells.Select(cs => new CharacterSpellItemDto
            {
                SpellId = cs.SpellId,
                Name = cs.Spell?.Name ?? string.Empty,
                Level = cs.Spell?.Level ?? 0,
                School = cs.Spell?.School ?? string.Empty,
                IsPrepared = cs.IsPrepared,
            }).ToList(),
            Skills = character.CharacterSkills.Select(cs =>
            {
                var ability = cs.Skill?.Ability ?? AbilityType.Str;
                var baseMod = Modifier(scores.GetValueOrDefault(ability, 10));
                var mod = baseMod + (cs.IsProficient ? prof : 0) + cs.BonusModifier;
                return new CharacterSkillItemDto
                {
                    SkillId = cs.SkillId,
                    Name = cs.Skill?.Name ?? string.Empty,
                    Ability = ability,
                    IsProficient = cs.IsProficient,
                    Modifier = mod,
                };
            }).OrderBy(s => s.Name).ToList(),
            InventoryItems = character.Inventory?.Items.Select(i => new InventoryItemDto
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                Quantity = i.Quantity,
                Weight = i.Weight,
                IsEquipped = i.IsEquipped,
            }).ToList() ?? [],
            SavingThrows = Enum.GetValues<AbilityType>().Select(a =>
            {
                var baseMod = Modifier(scores[a]);
                return new SavingThrowDto
                {
                    Ability = a,
                    Name = a.ToString(),
                    Modifier = baseMod,
                    IsProficient = false,
                };
            }).ToList(),
        };
    }

    private static CharacterListItemDto MapToListItem(Character character) => new()
    {
        Id = character.Id,
        Name = character.Name,
        Level = character.Level,
        RaceName = character.Race?.Name ?? string.Empty,
        ClassName = character.CharacterClass?.Name ?? string.Empty,
        UpdatedAt = character.UpdatedAt
    };
}
