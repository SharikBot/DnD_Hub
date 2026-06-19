using DnDCharacterManager.Core.Enums;

namespace DnDCharacterManager.Core.DTOs;

public class MonsterDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string ChallengeRating { get; set; } = string.Empty;

    public CreatureType CreatureType { get; set; }

    public int ArmorClass { get; set; }

    public int HitPoints { get; set; }

    public Dictionary<AbilityType, int> AbilityScores { get; set; } = new();

    public string Description { get; set; } = string.Empty;
}