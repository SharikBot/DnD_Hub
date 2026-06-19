using DnDCharacterManager.Core.DTOs;
using DnDCharacterManager.Core.Entities;
using DnDCharacterManager.Core.Enums;

namespace DnDCharacterManager.Core.Patterns.Factory;

public class CharacterFactory : ICharacterFactory
{
    public Character Create(CreateCharacterDto dto, Dictionary<AbilityType, int> abilityScores)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ArgumentNullException.ThrowIfNull(abilityScores);

        var character = new Character
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            UserId = dto.UserId,
            RaceId = dto.RaceId,
            CharacterClassId = dto.CharacterClassId,
            BackgroundId = dto.BackgroundId,
            Alignment = dto.Alignment,
            Backstory = dto.Backstory?.Trim(),
            Level = 1,
            Strength = GetScore(abilityScores, AbilityType.Str),
            Dexterity = GetScore(abilityScores, AbilityType.Dex),
            Constitution = GetScore(abilityScores, AbilityType.Con),
            Intelligence = GetScore(abilityScores, AbilityType.Int),
            Wisdom = GetScore(abilityScores, AbilityType.Wis),
            Charisma = GetScore(abilityScores, AbilityType.Cha),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var conModifier = (character.Constitution - 10) / 2;
        character.MaxHitPoints = Math.Max(1, 8 + conModifier);
        character.CurrentHitPoints = character.MaxHitPoints;
        character.ArmorClass = 10 + (character.Dexterity - 10) / 2;

        character.Inventory = new Inventory
        {
            Id = Guid.NewGuid(),
            CharacterId = character.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return character;
    }

    private static int GetScore(Dictionary<AbilityType, int> scores, AbilityType ability) =>
        scores.TryGetValue(ability, out var value) ? value : 10;
}
