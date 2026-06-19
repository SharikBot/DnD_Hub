using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DnDCharacterManager.Core.DTOs;
using DnDCharacterManager.Core.Enums;
using DnDCharacterManager.Desktop.Models;
using DnDCharacterManager.Desktop.Services;

namespace DnDCharacterManager.Desktop.ViewModels;

public partial class RulebookViewModel : ObservableObject
{
    private readonly IApiClient _apiClient;
    private readonly IDialogService _dialogService;

    public RulebookViewModel(IApiClient apiClient, IDialogService dialogService)
    {
        _apiClient = apiClient;
        _dialogService = dialogService;

        GlossaryTerms =
        [
            new GlossaryTerm { Term = "Проверка характеристики", Definition = "Бросок d20 + модификатор характеристики против Сл." },
            new GlossaryTerm { Term = "Преимущество", Definition = "Бросьте два d20 и используйте больший результат." },
            new GlossaryTerm { Term = "Недостаток", Definition = "Бросьте два d20 и используйте меньший результат." },
            new GlossaryTerm { Term = "Класс брони", Definition = "Показатель защиты персонажа от атак." },
            new GlossaryTerm { Term = "Инициатива", Definition = "Проверка Ловкости, определяющая порядок хода в бою." },
            new GlossaryTerm { Term = "Спасбросок", Definition = "d20 + модификатор + бонус мастерства (если владение) против Сл эффекта." },
            new GlossaryTerm { Term = "Концентрация", Definition = "При получении урона — спасбросок Телосложения Сл 10 или половина урона." },
            new GlossaryTerm { Term = "Владение", Definition = "На 1 уровне бонус мастерства +2. Добавляется к владениям." }
        ];

        _ = SearchAsync();
    }

    public ObservableCollection<RuleDto> Rules { get; } = [];
    public ObservableCollection<RuleContentSegment> RuleContentSegments { get; } = [];
    public ObservableCollection<GlossaryTerm> GlossaryTerms { get; }
    public IReadOnlyList<RuleCategory> Categories { get; } = Enum.GetValues<RuleCategory>().ToList();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private RuleCategory? _selectedCategory;

    [ObservableProperty]
    private RuleDto? _selectedRule;

    [ObservableProperty]
    private GlossaryTerm? _selectedGlossaryTerm;

    [ObservableProperty]
    private bool _isLoading;

    partial void OnSelectedRuleChanged(RuleDto? value) => BuildContentSegments(value?.Content);

    [RelayCommand]
    private async Task SearchAsync()
    {
        IsLoading = true;
        try
        {
            IReadOnlyList<RuleDto> results = SelectedCategory is RuleCategory category
                ? await _apiClient.GetRulesByCategoryAsync(category)
                : await _apiClient.SearchRulesAsync(SearchText);

            Rules.Clear();
            foreach (var rule in results.Where(r => string.IsNullOrWhiteSpace(SearchText)
                                                     || r.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                                                     || r.Content.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
            {
                Rules.Add(rule);
            }

            SelectedRule = Rules.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Ошибка загрузки правил: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void SelectGlossaryTerm(GlossaryTerm? term)
    {
        if (term is null)
        {
            return;
        }

        SelectedGlossaryTerm = term;
        _dialogService.ShowInfo($"{term.Term}\n\n{term.Definition}");
    }

    [RelayCommand]
    private void OpenLinkedTerm(GlossaryTerm? term)
    {
        if (term is null)
        {
            return;
        }

        SelectedGlossaryTerm = term;
        _dialogService.ShowInfo($"{term.Term}\n\n{term.Definition}");
    }

    partial void OnSelectedCategoryChanged(RuleCategory? value)
    {
        _ = SearchAsync();
    }

    private void BuildContentSegments(string? content)
    {
        RuleContentSegments.Clear();
        if (string.IsNullOrWhiteSpace(content))
        {
            return;
        }

        var terms = GlossaryTerms
            .OrderByDescending(t => t.Term.Length)
            .ToList();

        var index = 0;
        while (index < content.Length)
        {
            var match = FindNextTermMatch(content, index, terms, out var matchedTerm);
            if (match is null || matchedTerm is null)
            {
                RuleContentSegments.Add(new RuleContentSegment { Text = content[index..] });
                break;
            }

            if (match.Value.start > index)
            {
                RuleContentSegments.Add(new RuleContentSegment
                {
                    Text = content[index..match.Value.start]
                });
            }

            RuleContentSegments.Add(new RuleContentSegment
            {
                Text = content.Substring(match.Value.start, match.Value.length),
                LinkedTerm = matchedTerm
            });

            index = match.Value.start + match.Value.length;
        }
    }

    private static (int start, int length)? FindNextTermMatch(
        string content,
        int startIndex,
        IReadOnlyList<GlossaryTerm> terms,
        out GlossaryTerm? matchedTerm)
    {
        matchedTerm = null;
        (int start, int length)? best = null;

        foreach (var term in terms)
        {
            var idx = content.IndexOf(term.Term, startIndex, StringComparison.OrdinalIgnoreCase);
            if (idx < 0)
            {
                continue;
            }

            if (best is null || idx < best.Value.start)
            {
                best = (idx, term.Term.Length);
                matchedTerm = term;
            }
        }

        return best;
    }
}
