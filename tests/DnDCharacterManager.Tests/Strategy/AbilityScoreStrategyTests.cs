using DnDCharacterManager.Core.Enums;
using DnDCharacterManager.Core.Patterns.Strategy;
using DnDCharacterManager.Tests.TestHelpers;

namespace DnDCharacterManager.Tests.Strategy;

public class AbilityScoreStrategyTests
{
    [Fact]
    public void StandardArray_ReturnsDefaultOrder_WhenNoScoresProvided()
    {
        var strategy = new StandardArrayStrategy();

        var scores = strategy.GenerateScores();

        Assert.Equal("Стандартный массив", strategy.MethodName);
        Assert.Equal(15, scores[AbilityType.Str]);
        Assert.Equal(14, scores[AbilityType.Dex]);
        Assert.Equal(13, scores[AbilityType.Con]);
        Assert.Equal(12, scores[AbilityType.Int]);
        Assert.Equal(10, scores[AbilityType.Wis]);
        Assert.Equal(8, scores[AbilityType.Cha]);
    }

    [Fact]
    public void StandardArray_AcceptsValidPermutation()
    {
        var strategy = new StandardArrayStrategy();
        var requested = new Dictionary<AbilityType, int>
        {
            [AbilityType.Str] = 8,
            [AbilityType.Dex] = 15,
            [AbilityType.Con] = 14,
            [AbilityType.Int] = 13,
            [AbilityType.Wis] = 12,
            [AbilityType.Cha] = 10
        };

        var scores = strategy.GenerateScores(requested);

        Assert.Equal(requested, scores);
    }

    [Fact]
    public void StandardArray_Throws_WhenValuesAreInvalid()
    {
        var strategy = new StandardArrayStrategy();
        var invalid = new Dictionary<AbilityType, int>
        {
            [AbilityType.Str] = 16,
            [AbilityType.Dex] = 14,
            [AbilityType.Con] = 13,
            [AbilityType.Int] = 12,
            [AbilityType.Wis] = 10,
            [AbilityType.Cha] = 8
        };

        Assert.Throws<ArgumentException>(() => strategy.GenerateScores(invalid));
    }

    [Fact]
    public void Roll4D6_GeneratesSixScores_InValidRange()
    {
        var strategy = new Roll4D6Strategy(new SequenceRandom(6, 5, 4, 1, 3, 3, 3, 3));

        var scores = strategy.GenerateScores();

        Assert.Equal(6, scores.Count);
        Assert.All(scores.Values, value => Assert.InRange(value, 3, 18));
        Assert.Equal("4d6 (отбросить наименьший)", strategy.MethodName);
    }

    [Fact]
    public void Roll4D6_DropsLowestDie_WhenSequenceIsKnown()
    {
        var strategy = new Roll4D6Strategy(new SequenceRandom(6, 5, 4, 1));

        var scores = strategy.GenerateScores();

        Assert.All(scores.Values, value => Assert.Equal(15, value));
    }

    [Fact]
    public void Roll4D6_Throws_WhenPresetScoresProvided()
    {
        var strategy = new Roll4D6Strategy();
        var preset = new Dictionary<AbilityType, int> { [AbilityType.Str] = 15 };

        Assert.Throws<ArgumentException>(() => strategy.GenerateScores(preset));
    }

    [Fact]
    public void PointBuy_ReturnsDefaultDistribution_WhenNoScoresProvided()
    {
        var strategy = new PointBuyStrategy();

        var scores = strategy.GenerateScores();

        Assert.Equal(15, scores[AbilityType.Str]);
        Assert.Equal(14, scores[AbilityType.Dex]);
        Assert.Equal(13, scores[AbilityType.Con]);
        Assert.Equal("Покупка очков (27)", strategy.MethodName);
    }

    [Fact]
    public void PointBuy_AcceptsValidDistribution()
    {
        var strategy = new PointBuyStrategy();
        var requested = new Dictionary<AbilityType, int>
        {
            [AbilityType.Str] = 15,
            [AbilityType.Dex] = 14,
            [AbilityType.Con] = 13,
            [AbilityType.Int] = 12,
            [AbilityType.Wis] = 10,
            [AbilityType.Cha] = 8
        };

        var scores = strategy.GenerateScores(requested);

        Assert.Equal(requested, scores);
    }

    [Fact]
    public void PointBuy_Throws_WhenBudgetExceeded()
    {
        var overBudget = new Dictionary<AbilityType, int>
        {
            [AbilityType.Str] = 15,
            [AbilityType.Dex] = 15,
            [AbilityType.Con] = 15,
            [AbilityType.Int] = 15,
            [AbilityType.Wis] = 15,
            [AbilityType.Cha] = 15
        };

        Assert.Throws<ArgumentException>(() => PointBuyStrategy.ValidateScores(overBudget));
    }

    [Fact]
    public void PointBuy_Throws_WhenScoreOutOfRange()
    {
        var invalid = new Dictionary<AbilityType, int>
        {
            [AbilityType.Str] = 7,
            [AbilityType.Dex] = 10,
            [AbilityType.Con] = 10,
            [AbilityType.Int] = 10,
            [AbilityType.Wis] = 10,
            [AbilityType.Cha] = 10
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => PointBuyStrategy.ValidateScores(invalid));
    }

    [Theory]
    [InlineData(8, 0)]
    [InlineData(13, 5)]
    [InlineData(15, 9)]
    public void PointBuy_GetPointCost_ReturnsExpectedValues(int score, int expectedCost)
    {
        Assert.Equal(expectedCost, PointBuyStrategy.GetPointCost(score));
    }
}
