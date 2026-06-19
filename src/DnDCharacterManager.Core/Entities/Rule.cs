using DnDCharacterManager.Core.Common;
using DnDCharacterManager.Core.Enums;

namespace DnDCharacterManager.Core.Entities;

public class Rule : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public RuleCategory Category { get; set; }

    public string Source { get; set; } = string.Empty;
}
