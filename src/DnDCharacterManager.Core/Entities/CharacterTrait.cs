namespace DnDCharacterManager.Core.Entities;

public class CharacterTrait
{
    public Guid CharacterId { get; set; }

    public Guid TraitId { get; set; }

    public string? Notes { get; set; }

    public Character Character { get; set; } = null!;

    public Trait Trait { get; set; } = null!;
}
