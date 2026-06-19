using DnDCharacterManager.Core.Enums;
using DnDCharacterManager.Core.Interfaces.Services;

namespace DnDCharacterManager.Core.Patterns.Strategy;

public class Roll4D6Strategy : IAbilityScoreStrategy
{
    private readonly Random _random;

    public Roll4D6Strategy(Random? random = null)
    {
        _random = random ?? Random.Shared;
    }

    public string MethodName => "4d6 (отбросить наименьший)";

    public Dictionary<AbilityType, int> GenerateScores(Dictionary<AbilityType, int>? requestedScores = null)
    {
        if (requestedScores is { Count: > 0 })
        {
            throw new ArgumentException(
                "Метод 4d6 не принимает заранее заданные значения характеристик.",
                nameof(requestedScores));
        }

        return Enum.GetValues<AbilityType>()
            .ToDictionary(ability => ability, _ => Roll4D6DropLowest());
    }

    private int Roll4D6DropLowest()
    {
        var rolls = Enumerable.Range(0, 4).Select(_ => _random.Next(1, 7)).OrderByDescending(r => r).ToArray();
        return rolls[0] + rolls[1] + rolls[2];
    }
}
