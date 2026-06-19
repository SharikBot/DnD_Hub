namespace DnDCharacterManager.Core.Entities;

public class CharacterSkill
{
    public Guid CharacterId { get; set; }

    public Guid SkillId { get; set; }

    public bool IsProficient { get; set; }

    public bool HasExpertise { get; set; }

    public int BonusModifier { get; set; }

    public Character Character { get; set; } = null!;

    public Skill Skill { get; set; } = null!;
}
