namespace DnDCharacterManager.Core.Entities;

public class CharacterSpell
{
    public Guid CharacterId { get; set; }

    public Guid SpellId { get; set; }

    public bool IsPrepared { get; set; }

    public bool IsCantrip { get; set; }

    public Character Character { get; set; } = null!;

    public Spell Spell { get; set; } = null!;
}
