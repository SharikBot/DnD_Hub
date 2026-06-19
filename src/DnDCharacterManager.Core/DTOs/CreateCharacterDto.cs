using DnDCharacterManager.Core.Enums;

namespace DnDCharacterManager.Core.DTOs;

public class CreateCharacterDto
{
    public string Name { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public Guid RaceId { get; set; }

    public Guid CharacterClassId { get; set; }

    public Guid BackgroundId { get; set; }

    public AlignmentType Alignment { get; set; } = AlignmentType.TrueNeutral;

    public string? Backstory { get; set; }

    public AbilityScoreMethod AbilityScoreMethod { get; set; } = AbilityScoreMethod.StandardArray;

    public Dictionary<AbilityType, int>? AbilityScores { get; set; }

    public List<Guid> TraitIds { get; set; } = [];

    public List<Guid> SpellIds { get; set; } = [];

    public List<string> EquipmentItems { get; set; } = [];

    public string? PortraitPath { get; set; }
}

public enum AbilityScoreMethod
{
    StandardArray,

    Roll4D6,

    PointBuy
}
