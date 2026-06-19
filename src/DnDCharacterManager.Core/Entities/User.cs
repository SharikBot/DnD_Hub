using DnDCharacterManager.Core.Common;

namespace DnDCharacterManager.Core.Entities;

public class User : BaseEntity
{
    public string DisplayName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public ICollection<Character> Characters { get; set; } = new List<Character>();
}
