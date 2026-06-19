using DnDCharacterManager.Core.Enums;
using DnDCharacterManager.Core.Interfaces.Services;

namespace DnDCharacterManager.Core.Patterns.Strategy;

public class PointBuyStrategy : IAbilityScoreStrategy
{
    public const int PointBudget = 27;
    public const int MinScore = 8;
    public const int MaxScore = 15;

    private static readonly Dictionary<int, int> PointCosts = new()
    {
        [8] = 0,
        [9] = 1,
        [10] = 2,
        [11] = 3,
        [12] = 4,
        [13] = 5,
        [14] = 7,
        [15] = 9
    };

    public string MethodName => "Покупка очков (27)";

    public Dictionary<AbilityType, int> GenerateScores(Dictionary<AbilityType, int>? requestedScores = null)
    {
        if (requestedScores is null || requestedScores.Count != 6)
        {
            return GetDefaultDistribution();
        }

        ValidateScores(requestedScores);
        return new Dictionary<AbilityType, int>(requestedScores);
    }

    public static void ValidateScores(Dictionary<AbilityType, int> scores)
    {
        var totalCost = scores.Values.Sum(GetPointCost);

        if (totalCost > PointBudget)
        {
            throw new ArgumentException(
                $"Сумма очков ({totalCost}) превышает бюджет ({PointBudget}).",
                nameof(scores));
        }

        foreach (var score in scores.Values)
        {
            if (score < MinScore || score > MaxScore)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(scores),
                    $"Значение {score} вне диапазона {MinScore}–{MaxScore}.");
            }
        }
    }

    public static int GetPointCost(int score) =>
        PointCosts.TryGetValue(score, out var cost)
            ? cost
            : throw new ArgumentOutOfRangeException(nameof(score), $"Недопустимое значение: {score}.");

    private static Dictionary<AbilityType, int> GetDefaultDistribution() =>
        new()
        {
            [AbilityType.Str] = 15,
            [AbilityType.Dex] = 14,
            [AbilityType.Con] = 13,
            [AbilityType.Int] = 12,
            [AbilityType.Wis] = 10,
            [AbilityType.Cha] = 8
        };
}
