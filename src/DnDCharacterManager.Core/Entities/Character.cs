using DnDCharacterManager.Core.Common;
using DnDCharacterManager.Core.Enums;

namespace DnDCharacterManager.Core.Entities;

public class Character : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public int Level { get; set; } = 1;

    public AlignmentType Alignment { get; set; } = AlignmentType.TrueNeutral;

    public string? Backstory { get; set; }

    public string? PortraitPath { get; set; }

    public int Strength { get; set; } = 10;

    public int Dexterity { get; set; } = 10;

    public int Constitution { get; set; } = 10;

    public int Intelligence { get; set; } = 10;

    public int Wisdom { get; set; } = 10;

    public int Charisma { get; set; } = 10;

    public int CurrentHitPoints { get; set; }

    public int MaxHitPoints { get; set; }

    public int ArmorClass { get; set; } = 10;

    public int Speed { get; set; } = 30;

    public Guid UserId { get; set; }

    public Guid RaceId { get; set; }

    public Guid CharacterClassId { get; set; }

    public Guid BackgroundId { get; set; }

    public User User { get; set; } = null!;

    public Race Race { get; set; } = null!;

    public CharacterClass CharacterClass { get; set; } = null!;

    public Background Background { get; set; } = null!;

    public Inventory? Inventory { get; set; }

    public ICollection<CharacterTrait> CharacterTraits { get; set; } = new List<CharacterTrait>();

    public ICollection<CharacterSkill> CharacterSkills { get; set; } = new List<CharacterSkill>();

    public ICollection<CharacterSpell> CharacterSpells { get; set; } = new List<CharacterSpell>();
}
