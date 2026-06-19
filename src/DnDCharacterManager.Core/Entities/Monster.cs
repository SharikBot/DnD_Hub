using DnDCharacterManager.Core.Common;
using DnDCharacterManager.Core.Enums;

namespace DnDCharacterManager.Core.Entities;

public class Monster : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string ChallengeRating { get; set; } = "0";

    public CreatureType CreatureType { get; set; }

    public int ArmorClass { get; set; }

    public int HitPoints { get; set; }

    public int Speed { get; set; } = 30;

    public int Strength { get; set; }

    public int Dexterity { get; set; }

    public int Constitution { get; set; }

    public int Intelligence { get; set; }

    public int Wisdom { get; set; }

    public int Charisma { get; set; }

    public string Description { get; set; } = string.Empty;
}
