using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DnDCharacterManager.Core.DTOs;
using DnDCharacterManager.Core.Enums;
using DnDCharacterManager.Desktop.Services;

namespace DnDCharacterManager.Desktop.ViewModels;

public partial class BestiaryViewModel : ObservableObject
{
    private readonly IApiClient _apiClient;
    private readonly IDialogService _dialogService;

    public BestiaryViewModel(IApiClient apiClient, IDialogService dialogService)
    {
        _apiClient = apiClient;
        _dialogService = dialogService;
        _ = SearchAsync();
    }

    public ObservableCollection<MonsterDto> Monsters { get; } = [];
    public IReadOnlyList<CreatureType> CreatureTypes { get; } = Enum.GetValues<CreatureType>().ToList();
    public IReadOnlyList<string> SortOptions { get; } = ["Имя", "CR", "Тип"];

    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private CreatureType? _selectedCreatureType;
    [ObservableProperty] private MonsterDto? _selectedMonster;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _selectedSort = "Имя";

    [RelayCommand]
    private async Task SearchAsync()
    {
        IsLoading = true;
        try
        {
            var results = await _apiClient.SearchMonstersAsync(SearchText);
            Monsters.Clear();

            var filtered = results
                .Where(m => SelectedCreatureType is null || m.CreatureType == SelectedCreatureType)
                .ToList();

            IEnumerable<MonsterDto> sorted = SelectedSort switch
            {
                "CR" => filtered.OrderBy(m => ParseCr(m.ChallengeRating)),
                "Тип" => filtered.OrderBy(m => m.CreatureType).ThenBy(m => m.Name),
                _ => filtered.OrderBy(m => m.Name)
            };

            foreach (var monster in sorted)
                Monsters.Add(monster);

            SelectedMonster = Monsters.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Ошибка поиска монстров: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSelectedCreatureTypeChanged(CreatureType? value) => _ = SearchAsync();
    partial void OnSelectedSortChanged(string value) => _ = SearchAsync();

    private static double ParseCr(string cr)
    {
        if (string.IsNullOrWhiteSpace(cr)) return 0;
        if (cr.Contains('/'))
        {
            var parts = cr.Split('/');
            if (parts.Length == 2 && double.TryParse(parts[0], out var num) && double.TryParse(parts[1], out var den) && den > 0)
                return num / den;
        }
        return double.TryParse(cr, out var value) ? value : 0;
    }
}
