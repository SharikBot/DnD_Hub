using DnDCharacterManager.Core.Enums;
using DnDCharacterManager.Core.Interfaces.Services;

namespace DnDCharacterManager.Core.Patterns.Strategy;

public class StandardArrayStrategy : IAbilityScoreStrategy
{
    private static readonly int[] StandardValues = [15, 14, 13, 12, 10, 8];

    public string MethodName => "Стандартный массив";

    public Dictionary<AbilityType, int> GenerateScores(Dictionary<AbilityType, int>? requestedScores = null)
    {
        if (requestedScores is null || requestedScores.Count != 6)
        {
            return AssignDefaultOrder();
        }

        var values = requestedScores.Values.OrderByDescending(v => v).ToArray();
        if (!ValuesMatchStandardSet(values))
        {
            throw new ArgumentException(
                "Для стандартного массива допустимы только значения 15, 14, 13, 12, 10, 8.",
                nameof(requestedScores));
        }

        return new Dictionary<AbilityType, int>(requestedScores);
    }

    private static Dictionary<AbilityType, int> AssignDefaultOrder() =>
        new()
        {
            [AbilityType.Str] = 15,
            [AbilityType.Dex] = 14,
            [AbilityType.Con] = 13,
            [AbilityType.Int] = 12,
            [AbilityType.Wis] = 10,
            [AbilityType.Cha] = 8
        };

    private static bool ValuesMatchStandardSet(int[] sortedValues)
    {
        var expected = StandardValues.OrderByDescending(v => v).ToArray();
        return sortedValues.SequenceEqual(expected);
    }
}
