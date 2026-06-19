using DnDCharacterManager.Core.Enums;

namespace DnDCharacterManager.Core.DTOs;

public class CharacterDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
    public AlignmentType Alignment { get; set; }
    public string? Backstory { get; set; }
    public string? PortraitPath { get; set; }
    public Dictionary<AbilityType, int> AbilityScores { get; set; } = new();
    public int CurrentHitPoints { get; set; }
    public int MaxHitPoints { get; set; }
    public int ArmorClass { get; set; }
    public int Speed { get; set; }
    public int InitiativeBonus { get; set; }
    public int ProficiencyBonus { get; set; }
    public string RaceName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string BackgroundName { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<CharacterTraitItemDto> Traits { get; set; } = [];
    public List<CharacterSpellItemDto> Spells { get; set; } = [];
    public List<CharacterSkillItemDto> Skills { get; set; } = [];
    public List<InventoryItemDto> InventoryItems { get; set; } = [];
    public List<SavingThrowDto> SavingThrows { get; set; } = [];
}
