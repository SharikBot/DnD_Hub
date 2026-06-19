using DnDCharacterManager.Core.DTOs;
using DnDCharacterManager.Core.Entities;
using DnDCharacterManager.Core.Enums;

namespace DnDCharacterManager.Core.Patterns.Factory;

public interface ICharacterFactory
{
    Character Create(CreateCharacterDto dto, Dictionary<AbilityType, int> abilityScores);
}
