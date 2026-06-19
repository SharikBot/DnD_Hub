using DnDCharacterManager.Core.Enums;

namespace DnDCharacterManager.Core.DTOs;

public class CharacterTraitItemDto
{
    public Guid TraitId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class CharacterSpellItemDto
{
    public Guid SpellId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
    public string School { get; set; } = string.Empty;
    public bool IsPrepared { get; set; }
}

public class CharacterSkillItemDto
{
    public Guid SkillId { get; set; }
    public string Name { get; set; } = string.Empty;
    public AbilityType Ability { get; set; }
    public bool IsProficient { get; set; }
    public int Modifier { get; set; }
}

public class InventoryItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Weight { get; set; }
    public bool IsEquipped { get; set; }
}

public class SavingThrowDto
{
    public AbilityType Ability { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Modifier { get; set; }
    public bool IsProficient { get; set; }
}
