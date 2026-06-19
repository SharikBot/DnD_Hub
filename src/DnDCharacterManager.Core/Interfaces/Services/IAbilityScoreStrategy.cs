using DnDCharacterManager.Core.Enums;

namespace DnDCharacterManager.Core.Interfaces.Services;

public interface IAbilityScoreStrategy
{
    string MethodName { get; }

    Dictionary<AbilityType, int> GenerateScores(Dictionary<AbilityType, int>? requestedScores = null);
}
