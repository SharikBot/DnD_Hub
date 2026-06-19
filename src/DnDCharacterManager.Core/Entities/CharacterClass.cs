using DnDCharacterManager.Core.Common;

namespace DnDCharacterManager.Core.Entities;

public class CharacterClass : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string HitDie { get; set; } = "d8";

    public string PrimaryAbility { get; set; } = string.Empty;

    public int BaseArmorClass { get; set; } = 10;

    public ICollection<Character> Characters { get; set; } = new List<Character>();
}