using DnDCharacterManager.Core.Common;

namespace DnDCharacterManager.Core.Entities;

public class Trait : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;

    public ICollection<CharacterTrait> CharacterTraits { get; set; } = new List<CharacterTrait>();
}
