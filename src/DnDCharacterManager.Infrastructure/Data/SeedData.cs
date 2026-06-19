namespace DnDCharacterManager.Infrastructure.Data;

public static class SeedData
{
    public static readonly Guid DemoUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public static readonly Guid RaceHuman = Guid.Parse("11111111-1111-1111-1111-111111111101");
    public static readonly Guid RaceElf = Guid.Parse("11111111-1111-1111-1111-111111111102");
    public static readonly Guid RaceDwarf = Guid.Parse("11111111-1111-1111-1111-111111111103");
    public static readonly Guid RaceHalfling = Guid.Parse("11111111-1111-1111-1111-111111111104");
    public static readonly Guid RaceDragonborn = Guid.Parse("11111111-1111-1111-1111-111111111105");
    public static readonly Guid RaceTiefling = Guid.Parse("11111111-1111-1111-1111-111111111106");

    public static readonly Guid ClassFighter = Guid.Parse("22222222-2222-2222-2222-222222222201");
    public static readonly Guid ClassWizard = Guid.Parse("22222222-2222-2222-2222-222222222202");
    public static readonly Guid ClassRogue = Guid.Parse("22222222-2222-2222-2222-222222222203");
    public static readonly Guid ClassCleric = Guid.Parse("22222222-2222-2222-2222-222222222204");
    public static readonly Guid ClassBarbarian = Guid.Parse("22222222-2222-2222-2222-222222222205");
    public static readonly Guid ClassRanger = Guid.Parse("22222222-2222-2222-2222-222222222206");

    public static readonly Guid BgAcolyte = Guid.Parse("33333333-3333-3333-3333-333333333301");
    public static readonly Guid BgSoldier = Guid.Parse("33333333-3333-3333-3333-333333333302");
    public static readonly Guid BgSage = Guid.Parse("33333333-3333-3333-3333-333333333303");
    public static readonly Guid BgCriminal = Guid.Parse("33333333-3333-3333-3333-333333333304");
}