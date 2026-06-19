using DnDCharacterManager.Core.Common;
using DnDCharacterManager.Core.Enums;

namespace DnDCharacterManager.Core.Entities;

public class Skill : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public AbilityType Ability { get; set; }

    public string Description { get; set; } = string.Empty;

    public ICollection<CharacterSkill> CharacterSkills { get; set; } = new List<CharacterSkill>();
}
