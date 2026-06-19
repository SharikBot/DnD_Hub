using DnDCharacterManager.Core.Enums;

namespace DnDCharacterManager.Desktop.Helpers;

public static class AlignmentHelper
{
    public static string GetDisplayName(AlignmentType alignment) => alignment switch
    {
        AlignmentType.LawfulGood => "Законопослушный добрый",
        AlignmentType.NeutralGood => "Нейтральный добрый",
        AlignmentType.ChaoticGood => "Хаотичный добрый",
        AlignmentType.LawfulNeutral => "Законопослушный нейтральный",
        AlignmentType.TrueNeutral => "Истинно нейтральный",
        AlignmentType.ChaoticNeutral => "Хаотичный нейтральный",
        AlignmentType.LawfulEvil => "Законопослушный злой",
        AlignmentType.NeutralEvil => "Нейтральный злой",
        AlignmentType.ChaoticEvil => "Хаотичный злой",
        AlignmentType.Unaligned => "Без мировоззрения",
        _ => alignment.ToString()
    };

    public static IReadOnlyList<AlignmentType> All { get; } = Enum.GetValues<AlignmentType>().ToList();
}