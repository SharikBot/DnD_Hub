using DnDCharacterManager.Core.Common;

namespace DnDCharacterManager.Core.Entities;

public class Race : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int StrengthBonus { get; set; }

    public int DexterityBonus { get; set; }

    public int ConstitutionBonus { get; set; }

    public int IntelligenceBonus { get; set; }

    public int WisdomBonus { get; set; }

    public int CharismaBonus { get; set; }

    public int BaseSpeed { get; set; } = 30;

    public string Size { get; set; } = "Medium";

    public ICollection<Character> Characters { get; set; } = new List<Character>();
}
