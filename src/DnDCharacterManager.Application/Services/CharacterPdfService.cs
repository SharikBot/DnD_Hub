using DnDCharacterManager.Core.Enums;
using DnDCharacterManager.Core.Interfaces.Repositories;
using DnDCharacterManager.Core.Interfaces.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DnDCharacterManager.Application.Services;

public class CharacterPdfService : ICharacterPdfService
{
    private readonly IUnitOfWork _unitOfWork;

    public CharacterPdfService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerateCharacterSheetAsync(Guid characterId, CancellationToken cancellationToken = default)
    {
        var character = await _unitOfWork.Characters.GetByIdWithDetailsAsync(characterId, cancellationToken)
            ?? throw new KeyNotFoundException($"Character {characterId} was not found.");

        var proficiency = 2 + (character.Level - 1) / 4;
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(24);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Background(Colors.Grey.Lighten3).Padding(8).Row(row =>
                {
                    if (!string.IsNullOrWhiteSpace(character.PortraitPath) && File.Exists(character.PortraitPath))
                    {
                        row.ConstantItem(72).Height(72).Image(character.PortraitPath).FitArea();
                    }

                    row.RelativeItem().PaddingLeft(8).Column(col =>
                    {
                        col.Item().Text("D&D 5e Character Sheet").FontSize(16).Bold().FontColor(Colors.Red.Darken3);
                        col.Item().Text($"{character.Name}").FontSize(14).Bold();
                        col.Item().Text(
                            $"Level {character.Level} {character.CharacterClass?.Name} ({character.Race?.Name}) · {character.Alignment}");
                    });
                });

                page.Content().PaddingVertical(8).Column(column =>
                {
                    column.Spacing(6);

                    column.Item().Row(row =>
                    {
                        foreach (var ability in new[] { ("STR", character.Strength), ("DEX", character.Dexterity),
                                     ("CON", character.Constitution), ("INT", character.Intelligence),
                                     ("WIS", character.Wisdom), ("CHA", character.Charisma) })
                        {
                            row.RelativeItem().Border(1).BorderColor(Colors.Grey.Medium).Padding(4).Column(c =>
                            {
                                c.Item().AlignCenter().Text(ability.Item1).Bold().FontSize(8);
                                c.Item().AlignCenter().Text(ability.Item2.ToString()).FontSize(14).Bold();
                                c.Item().AlignCenter().Text(Modifier(ability.Item2)).FontSize(10);
                            });
                        }
                    });

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Border(1).Padding(6).Column(c =>
                        {
                            c.Item().Text("Combat").Bold().Underline();
                            c.Item().Text($"Armor Class: {character.ArmorClass}");
                            c.Item().Text($"Hit Points: {character.CurrentHitPoints}/{character.MaxHitPoints}");
                            c.Item().Text($"Speed: {character.Speed} ft.");
                            c.Item().Text($"Proficiency Bonus: +{proficiency}");
                            c.Item().Text($"Initiative: {Modifier(character.Dexterity)}");
                        });

                        row.RelativeItem().Border(1).Padding(6).Column(c =>
                        {
                            c.Item().Text("Character Details").Bold().Underline();
                            c.Item().Text($"Background: {character.Background?.Name ?? "—"}");
                            c.Item().Text($"Class: {character.CharacterClass?.Name ?? "—"}");
                            c.Item().Text($"Race: {character.Race?.Name ?? "—"}");
                        });
                    });

                    column.Item().Border(1).Padding(6).Column(c =>
                    {
                        c.Item().Text("Skills & Saving Throws").Bold().Underline();
                        if (character.CharacterSkills.Count == 0)
                        {
                            c.Item().Text("—");
                        }
                        else
                        {
                            foreach (var skill in character.CharacterSkills)
                            {
                                var mod = Modifier(GetAbilityScore(character, skill.Skill.Ability));
                                var prof = skill.IsProficient ? $"+{proficiency}" : "+0";
                                c.Item().Text($"• {skill.Skill.Name}: {mod} (prof {prof})");
                            }
                        }
                    });

                    column.Item().Border(1).Padding(6).Column(c =>
                    {
                        c.Item().Text("Features & Traits").Bold().Underline();
                        if (character.CharacterTraits.Count == 0)
                        {
                            c.Item().Text("—");
                        }
                        else
                        {
                            foreach (var trait in character.CharacterTraits)
                            {
                                c.Item().Text($"• {trait.Trait.Name}");
                            }
                        }
                    });
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("DnD Character Manager — Page 1 — ");
                    text.Span(DateTime.UtcNow.ToString("yyyy-MM-dd")).Italic();
                });
            });

            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(24);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Text($"{character.Name} — Spells & Inventory").FontSize(14).Bold();

                page.Content().PaddingVertical(8).Column(column =>
                {
                    column.Spacing(6);

                    column.Item().Border(1).Padding(6).Column(c =>
                    {
                        c.Item().Text("Known Spells").Bold().Underline();
                        if (character.CharacterSpells.Count == 0)
                        {
                            c.Item().Text("—");
                        }
                        else
                        {
                            foreach (var spell in character.CharacterSpells)
                            {
                                c.Item().Text($"• {spell.Spell.Name} (level {spell.Spell.Level}) — {spell.Spell.School}");
                            }
                        }
                    });

                    column.Item().Border(1).Padding(6).Column(c =>
                    {
                        c.Item().Text("Inventory").Bold().Underline();
                        var items = character.Inventory?.Items;
                        if (items is null || items.Count == 0)
                        {
                            c.Item().Text("—");
                        }
                        else
                        {
                            foreach (var item in items)
                            {
                                c.Item().Text($"• {item.Name} x{item.Quantity}");
                            }
                        }
                    });

                    column.Item().Border(1).Padding(6).Column(c =>
                    {
                        c.Item().Text("Backstory & Notes").Bold().Underline();
                        c.Item().Text(string.IsNullOrWhiteSpace(character.Backstory) ? "—" : character.Backstory);
                    });
                });

                page.Footer().AlignCenter().Text("DnD Character Manager — Page 2");
            });
        });

        return document.GeneratePdf();
    }

    private static int GetAbilityScore(Core.Entities.Character character, AbilityType ability) => ability switch
    {
        AbilityType.Str => character.Strength,
        AbilityType.Dex => character.Dexterity,
        AbilityType.Con => character.Constitution,
        AbilityType.Int => character.Intelligence,
        AbilityType.Wis => character.Wisdom,
        AbilityType.Cha => character.Charisma,
        _ => 10
    };

    private static string Modifier(int score)
    {
        var mod = (score - 10) / 2;
        return mod >= 0 ? $"+{mod}" : mod.ToString();
    }
}
