namespace DnDCharacterManager.Desktop.Models;

public class RuleContentSegment
{
    public string Text { get; init; } = string.Empty;
    public GlossaryTerm? LinkedTerm { get; init; }
    public bool IsLinkedTerm => LinkedTerm is not null;
}
