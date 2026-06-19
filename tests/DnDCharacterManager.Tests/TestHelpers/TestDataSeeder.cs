using DnDCharacterManager.Core.Entities;
using DnDCharacterManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DnDCharacterManager.Tests.TestHelpers;

internal sealed class TestSeedData
{
    public required User User { get; init; }
    public required Race Race { get; init; }
    public required CharacterClass CharacterClass { get; init; }
    public required Background Background { get; init; }
}

internal static class TestDataSeeder
{
    public static TestSeedData SeedReferenceData(AppDbContext context)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            DisplayName = "Test Player",
            Email = "test@example.com",
            PasswordHash = "hash"
        };

        var race = new Race
        {
            Id = Guid.NewGuid(),
            Name = "Human",
            Description = "Versatile race",
            BaseSpeed = 30
        };

        var characterClass = new CharacterClass
        {
            Id = Guid.NewGuid(),
            Name = "Fighter",
            Description = "Martial class",
            HitDie = "d10"
        };

        var background = new Background
        {
            Id = Guid.NewGuid(),
            Name = "Soldier",
            Description = "Military background"
        };

        context.Users.Add(user);
        context.Races.Add(race);
        context.CharacterClasses.Add(characterClass);
        context.Backgrounds.Add(background);
        context.SaveChanges();

        return new TestSeedData
        {
            User = user,
            Race = race,
            CharacterClass = characterClass,
            Background = background
        };
    }

    public static AppDbContext CreateInMemoryContext(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
