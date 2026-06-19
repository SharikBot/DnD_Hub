using DnDCharacterManager.Core.Entities;
using DnDCharacterManager.Core.Enums;
using DnDCharacterManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DnDCharacterManager.Infrastructure.Data;

public static class DevDataSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Races.AnyAsync(cancellationToken))
        {
            return;
        }

        var now = DateTime.UtcNow;

        db.Users.Add(new User
        {
            Id = SeedData.DemoUserId,
            DisplayName = "Демо игрок",
            Email = "demo@dnd.local",
            PasswordHash = "demo",
            CreatedAt = now,
            UpdatedAt = now,
        });

        db.Races.AddRange(
            Race(SeedData.RaceHuman, "Человек", "Универсальная раса — бонус +1 ко всем характеристикам.", 1, 1, 1, 1, 1, 1, 30),
            Race(SeedData.RaceElf, "Эльф", "Грациозные обитатели лесов с острыми чувствами и магией.", 0, 2, 0, 0, 0, 0, 30),
            Race(SeedData.RaceDwarf, "Дварф", "Выносливые горные кузнецы с устойчивостью к ядам.", 0, 0, 2, 0, 0, 0, 25),
            Race(SeedData.RaceHalfling, "Halfling", "Маленькие и удачливые путешественники.", 0, 2, 0, 0, 0, 0, 25),
            Race(SeedData.RaceDragonborn, "Dragonborn", "Гуманоиды с драконьим наследием и дыханием стихии.", 2, 0, 0, 0, 0, 1, 30),
            Race(SeedData.RaceTiefling, "Tiefling", "Потомки инфернального наследия с врождённой магией.", 0, 0, 0, 1, 0, 2, 30));

        db.CharacterClasses.AddRange(
            Class(SeedData.ClassFighter, "Воин", "Мастер оружия и доспехов.", "d10", "Strength", 16),
            Class(SeedData.ClassWizard, "Волшебник", "Мастер арканной магии.", "d6", "Intelligence", 10),
            Class(SeedData.ClassRogue, "Плут", "Скрытность, ловкость и точные удары.", "d8", "Dexterity", 12),
            Class(SeedData.ClassCleric, "Жрец", "Божественная магия и поддержка группы.", "d8", "Wisdom", 14),
            Class(SeedData.ClassBarbarian, "Варвар", "Яростный воин первой линии.", "d12", "Strength", 12),
            Class(SeedData.ClassRanger, "Следопыт", "Охотник и защитник диких земель.", "d10", "Dexterity", 14));

        db.Backgrounds.AddRange(
            Bg(SeedData.BgAcolyte, "Послушник", "Служитель храма.", "Insight, Religion", "Shelter of the Faithful"),
            Bg(SeedData.BgSoldier, "Солдат", "Военная служба.", "Athletics, Intimidation", "Military Rank"),
            Bg(SeedData.BgSage, "Мудрец", "Исследователь знаний.", "Arcana, History", "Researcher"),
            Bg(SeedData.BgCriminal, "Преступник", "Жизнь в тени закона.", "Deception, Stealth", "Criminal Contact"));

        db.Traits.AddRange(
            Trait("Тёмное зрение", "Видите в сумраке до 60 фт.", "Race"),
            Trait("Мастер на все руки", "Владение одним инструментом ремесленника.", "Background"),
            Trait("Бдительный", "Не можете быть застигнуты врасплох.", "Class"),
            Trait("Безрассудная атака", "Преимущество в атаке, но враги получают преимущество против вас.", "Class"),
            Trait("Скрытная атака", "Дополнительный урон при преимуществе или союзнике рядом.", "Class"),
            Trait("Божественный домен", "Доступ к заклинаниям домена.", "Class"));

        db.Spells.AddRange(
            Spell("Fire Bolt", 0, "Evocation", "1 action", "120 ft", "V, S", "Instantaneous", "Дальняя атака огнём."),
            Spell("Mage Hand", 0, "Conjuration", "1 action", "30 ft", "V, S", "1 minute", "Призрачная рука."),
            Spell("Magic Missile", 1, "Evocation", "1 action", "120 ft", "V, S", "Instantaneous", "Три стрелы силы без броска атаки."),
            Spell("Shield", 1, "Abjuration", "1 reaction", "Self", "V, S", "1 round", "+5 к КБ до начала следующего хода."),
            Spell("Cure Wounds", 1, "Evocation", "1 action", "Touch", "V, S", "Instantaneous", "Восстановление хитов."),
            Spell("Detect Magic", 1, "Divination", "1 action", "Self", "V, S", "10 minutes", "Ощущение магии вокруг."),
            Spell("Fireball", 3, "Evocation", "1 action", "150 ft", "V, S, M", "Instantaneous", "Взрыв огня 8d6."),
            Spell("Counterspell", 3, "Abjuration", "1 reaction", "60 ft", "S", "Instantaneous", "Прерывает заклинание."));

        db.Skills.AddRange(
            Skill("Athletics", AbilityType.Str, "Лазание, прыжки, плавание."),
            Skill("Acrobatics", AbilityType.Dex, "Баланс и акробатика."),
            Skill("Sleight of Hand", AbilityType.Dex, "Ловкость рук."),
            Skill("Stealth", AbilityType.Dex, "Скрытность."),
            Skill("Arcana", AbilityType.Int, "Знание магии."),
            Skill("History", AbilityType.Int, "Исторические знания."),
            Skill("Investigation", AbilityType.Int, "Расследование и поиск улик."),
            Skill("Nature", AbilityType.Int, "Природа и её тайны."),
            Skill("Religion", AbilityType.Int, "Религии и культы."),
            Skill("Animal Handling", AbilityType.Wis, "Управление животными."),
            Skill("Insight", AbilityType.Wis, "Чтение намерений."),
            Skill("Medicine", AbilityType.Wis, "Первая помощь и диагностика."),
            Skill("Perception", AbilityType.Wis, "Внимательность."),
            Skill("Survival", AbilityType.Wis, "Выживание в дикой природе."),
            Skill("Deception", AbilityType.Cha, "Обман."),
            Skill("Intimidation", AbilityType.Cha, "Запугивание."),
            Skill("Performance", AbilityType.Cha, "Выступления."),
            Skill("Persuasion", AbilityType.Cha, "Убеждение."));

        db.Monsters.AddRange(
            Monster("Гоблин", "1/4", CreatureType.Humanoid, 15, 7, 8, 14, 10, 10, 8, 8, "Мелкий злобный гуманоид."),
            Monster("Орк", "1/2", CreatureType.Humanoid, 13, 15, 16, 12, 12, 7, 7, 10, "Агрессивный воин племён."),
            Monster("Волк", "1/4", CreatureType.Beast, 13, 11, 12, 15, 12, 3, 4, 5, "Стая охотников с Pack Tactics."),
            Monster("Зомби", "1/4", CreatureType.Undead, 8, 22, 13, 6, 16, 3, 6, 5, "Медленная нежить."),
            Monster("Огр", "2", CreatureType.Monstrosity, 11, 59, 19, 8, 16, 5, 7, 7, "Грубая сила и дубина."),
            Monster("Мимик", "2", CreatureType.Monstrosity, 12, 58, 17, 12, 15, 5, 13, 8, "Притворяется сундуком."),
            Monster("Дракон молодой красный", "10", CreatureType.Dragon, 18, 178, 23, 10, 21, 14, 12, 19, "Огненное дыхание и полёт."),
            Monster("Бехолдер", "13", CreatureType.Aberration, 18, 180, 10, 14, 18, 17, 15, 17, "Летающий аберрационный тиран."));

        db.Rules.AddRange(
            Rule("Преимущество (Advantage)", RuleCategory.Combat, "При преимуществе бросайте d20 дважды и используйте больший результат.", "SRD"),
            Rule("Недостаток (Disadvantage)", RuleCategory.Combat, "При недостатке бросайте d20 дважды и используйте меньший результат.", "SRD"),
            Rule("Проверка характеристики", RuleCategory.General, "d20 + модификатор характеристики против Сл.", "SRD"),
            Rule("Спасбросок", RuleCategory.Combat, "d20 + модификатор + бонус мастерства (если владение) против Сл эффекта.", "SRD"),
            Rule("Инициатива", RuleCategory.Combat, "Проверка Ловкости определяет порядок хода в бою.", "SRD"),
            Rule("Класс брони (AC)", RuleCategory.Combat, "Показатель защиты от атак. Базово 10 + модификатор Ловкости.", "SRD"),
            Rule("Критическое попадание", RuleCategory.Combat, "Натуральная 20 на d20 — автоматическое попадание с удвоением костей урона.", "SRD"),
            Rule("Концентрация", RuleCategory.Magic, "При получении урона — спасбросок Телосложения Сл 10 или половина урона.", "SRD"),
            Rule("Отдых (Short Rest)", RuleCategory.General, "1 час: восстановление хитов через кости хитов.", "SRD"),
            Rule("Отдых (Long Rest)", RuleCategory.General, "8 часов: восстановление всех хитов и половины костей хитов.", "SRD"),
            Rule("Владение (Proficiency)", RuleCategory.General, "На 1 уровне бонус мастерства +2. Добавляется к владениям.", "SRD"),
            Rule("Дистанция и дальность", RuleCategory.Combat, "Атаки вне нормальной дальности — с помехой.", "SRD"));

        await db.SaveChangesAsync(cancellationToken);
    }

    private static Race Race(Guid id, string name, string desc, int str, int dex, int con, int intel, int wis, int cha, int speed) => new()
    {
        Id = id, Name = name, Description = desc,
        StrengthBonus = str, DexterityBonus = dex, ConstitutionBonus = con,
        IntelligenceBonus = intel, WisdomBonus = wis, CharismaBonus = cha,
        BaseSpeed = speed, Size = "Medium",
    };

    private static CharacterClass Class(Guid id, string name, string desc, string hitDie, string primary, int ac) => new()
    {
        Id = id, Name = name, Description = desc, HitDie = hitDie, PrimaryAbility = primary, BaseArmorClass = ac,
    };

    private static Background Bg(Guid id, string name, string desc, string skills, string feature) => new()
    {
        Id = id, Name = name, Description = desc, SkillProficiencies = skills, Feature = feature,
    };

    private static Trait Trait(string name, string desc, string source) => new()
    {
        Id = Guid.NewGuid(), Name = name, Description = desc, Source = source,
    };

    private static Spell Spell(string name, int level, string school, string time, string range, string comp, string dur, string desc) => new()
    {
        Id = Guid.NewGuid(), Name = name, Level = level, School = school,
        CastingTime = time, Range = range, Components = comp, Duration = dur, Description = desc,
    };

    private static Skill Skill(string name, AbilityType ability, string desc) => new()
    {
        Id = Guid.NewGuid(), Name = name, Ability = ability, Description = desc,
    };

    private static Monster Monster(string name, string cr, CreatureType type, int ac, int hp,
        int str, int dex, int con, int intel, int wis, int cha, string desc) => new()
    {
        Id = Guid.NewGuid(), Name = name, ChallengeRating = cr, CreatureType = type,
        ArmorClass = ac, HitPoints = hp,
        Strength = str, Dexterity = dex, Constitution = con,
        Intelligence = intel, Wisdom = wis, Charisma = cha,
        Description = desc,
    };

    private static Rule Rule(string title, RuleCategory cat, string content, string source) => new()
    {
        Id = Guid.NewGuid(), Title = title, Category = cat, Content = content, Source = source,
    };
}
