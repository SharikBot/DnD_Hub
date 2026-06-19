using DnDCharacterManager.Core.Common;

namespace DnDCharacterManager.Core.Entities;

public class Background : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string SkillProficiencies { get; set; } = string.Empty;

    public string Feature { get; set; } = string.Empty;

    public ICollection<Character> Characters { get; set; } = new List<Character>();
}
