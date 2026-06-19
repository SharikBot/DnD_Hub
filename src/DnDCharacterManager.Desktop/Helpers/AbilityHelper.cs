using DnDCharacterManager.Core.Enums;

namespace DnDCharacterManager.Desktop.Helpers;

public static class AbilityHelper
{
    public static string GetDisplayName(AbilityType ability) => ability switch
    {
        AbilityType.Str => "Сила",
        AbilityType.Dex => "Ловкость",
        AbilityType.Con => "Телосложение",
        AbilityType.Int => "Интеллект",
        AbilityType.Wis => "Мудрость",
        AbilityType.Cha => "Харизма",
        _ => ability.ToString()
    };

    public static int GetModifier(int score) => (score - 10) / 2;

    public static IReadOnlyList<AbilityType> All { get; } = Enum.GetValues<AbilityType>().ToList();
}