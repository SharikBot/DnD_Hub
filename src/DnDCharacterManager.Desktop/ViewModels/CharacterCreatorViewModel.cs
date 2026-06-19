using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DnDCharacterManager.Core.DTOs;
using DnDCharacterManager.Core.Enums;
using DnDCharacterManager.Desktop.Helpers;
using DnDCharacterManager.Desktop.Models;
using DnDCharacterManager.Desktop.Services;

namespace DnDCharacterManager.Desktop.ViewModels;

public partial class CharacterCreatorViewModel : ObservableObject
{
    private static readonly int[] StandardArray = [15, 14, 13, 12, 10, 8];
    private static readonly Guid DemoUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    private readonly IApiClient _apiClient;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;
    private readonly ISnackbarService _snackbar;

    public CharacterCreatorViewModel(
        IApiClient apiClient,
        IDialogService dialogService,
        INavigationService navigationService,
        ISnackbarService snackbar)
    {
        _apiClient = apiClient;
        _dialogService = dialogService;
        _navigationService = navigationService;
        _snackbar = snackbar;

        foreach (var ability in AbilityHelper.All)
        {
            AbilityEntries.Add(new AbilityScoreDisplay
            {
                Ability = ability,
                DisplayName = AbilityHelper.GetDisplayName(ability),
                Score = 10
            });
        }

        _ = LoadReferenceDataAsync();
    }

    public ObservableCollection<SelectionCardItem> Races { get; } = [];
    public ObservableCollection<SelectionCardItem> Classes { get; } = [];
    public ObservableCollection<SelectionCardItem> Backgrounds { get; } = [];
    public ObservableCollection<SelectionCardItem> Traits { get; } = [];
    public ObservableCollection<SelectionCardItem> Spells { get; } = [];
    public ObservableCollection<string> EquipmentItems { get; } = [];
    public ObservableCollection<AbilityScoreDisplay> AbilityEntries { get; } = [];
    public IReadOnlyList<AlignmentType> Alignments { get; } = AlignmentHelper.All;

    [ObservableProperty] private CreatorStep _currentStep = CreatorStep.Race;
    [ObservableProperty] private string _characterName = string.Empty;
    [ObservableProperty] private AlignmentType _selectedAlignment = AlignmentType.TrueNeutral;
    [ObservableProperty] private string _portraitPath = string.Empty;
    [ObservableProperty] private string _newEquipmentItem = string.Empty;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isSaving;
    [ObservableProperty] private string _stepTitle = "Выберите расу";

    public string CurrentStepDisplay => $"Шаг {(int)CurrentStep + 1} из 9";

    partial void OnCurrentStepChanged(CreatorStep value)
    {
        StepTitle = value switch
        {
            CreatorStep.Race => "Выберите расу",
            CreatorStep.Class => "Выберите класс",
            CreatorStep.Background => "Выберите предысторию",
            CreatorStep.Traits => "Выберите черты",
            CreatorStep.Spells => "Выберите заклинания",
            CreatorStep.Equipment => "Стартовое снаряжение",
            CreatorStep.Abilities => "Распределение характеристик",
            CreatorStep.Alignment => "Мировоззрение и имя",
            CreatorStep.Portrait => "Портрет персонажа",
            _ => string.Empty
        };
        OnPropertyChanged(nameof(CurrentStepDisplay));
    }

    [RelayCommand]
    private void SelectCard(SelectionCardItem? item)
    {
        if (item is null) return;

        var collection = CurrentStep switch
        {
            CreatorStep.Race => Races,
            CreatorStep.Class => Classes,
            CreatorStep.Background => Backgrounds,
            CreatorStep.Traits => Traits,
            CreatorStep.Spells => Spells,
            _ => null
        };

        if (collection is null)
        {
            item.IsSelected = !item.IsSelected;
            return;
        }

        if (CurrentStep is CreatorStep.Traits or CreatorStep.Spells)
        {
            item.IsSelected = !item.IsSelected;
            return;
        }

        foreach (var card in collection) card.IsSelected = card == item;
    }

    [RelayCommand]
    private void NextStep()
    {
        if (!ValidateCurrentStep()) return;
        if (CurrentStep < CreatorStep.Portrait) CurrentStep++;
    }

    [RelayCommand]
    private void PreviousStep()
    {
        if (CurrentStep > CreatorStep.Race) CurrentStep--;
    }

    [RelayCommand] private void ApplyStandardArray()
    {
        for (var i = 0; i < AbilityEntries.Count && i < StandardArray.Length; i++)
            AbilityEntries[i].Score = StandardArray[i];
    }

    [RelayCommand] private void AddEquipment()
    {
        if (string.IsNullOrWhiteSpace(NewEquipmentItem)) return;
        EquipmentItems.Add(NewEquipmentItem.Trim());
        NewEquipmentItem = string.Empty;
    }

    [RelayCommand] private void RemoveEquipment(string? item)
    {
        if (item is not null) EquipmentItems.Remove(item);
    }

    [RelayCommand] private void BrowsePortrait()
    {
        var path = _dialogService.ShowOpenFileDialog("Изображения|*.png;*.jpg;*.jpeg;*.bmp", "Выберите портрет");
        if (path is not null) PortraitPath = path;
    }

    [RelayCommand]
    private async Task SaveCharacterAsync()
    {
        if (!ValidateCurrentStep()) return;

        if (string.IsNullOrWhiteSpace(CharacterName))
        {
            _dialogService.ShowError("Введите имя персонажа.");
            return;
        }

        var race = Races.FirstOrDefault(r => r.IsSelected);
        var characterClass = Classes.FirstOrDefault(c => c.IsSelected);
        var background = Backgrounds.FirstOrDefault(b => b.IsSelected);

        if (race is null || characterClass is null || background is null)
        {
            _dialogService.ShowError("Выберите расу, класс и предысторию.");
            return;
        }

        IsSaving = true;
        try
        {
            var dto = new CreateCharacterDto
            {
                Name = CharacterName.Trim(),
                UserId = DemoUserId,
                RaceId = race.Id,
                CharacterClassId = characterClass.Id,
                BackgroundId = background.Id,
                Alignment = SelectedAlignment,
                AbilityScoreMethod = AbilityScoreMethod.StandardArray,
                AbilityScores = AbilityEntries.ToDictionary(e => e.Ability, e => e.Score),
                TraitIds = Traits.Where(t => t.IsSelected).Select(t => t.Id).ToList(),
                SpellIds = Spells.Where(s => s.IsSelected).Select(s => s.Id).ToList(),
                EquipmentItems = EquipmentItems.ToList(),
                PortraitPath = string.IsNullOrWhiteSpace(PortraitPath) ? null : PortraitPath,
            };

            await _apiClient.CreateCharacterAsync(dto);
            _snackbar.ShowSuccess("Персонаж успешно создан!");
            _navigationService.NavigateTo<CharactersListViewModel>();
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Не удалось создать персонажа: {ex.Message}");
        }
        finally
        {
            IsSaving = false;
        }
    }

    private bool ValidateCurrentStep()
    {
        switch (CurrentStep)
        {
            case CreatorStep.Race when Races.All(r => !r.IsSelected):
                _dialogService.ShowError("Выберите расу.");
                return false;
            case CreatorStep.Class when Classes.All(c => !c.IsSelected):
                _dialogService.ShowError("Выберите класс.");
                return false;
            case CreatorStep.Background when Backgrounds.All(b => !b.IsSelected):
                _dialogService.ShowError("Выберите предысторию.");
                return false;
            case CreatorStep.Alignment when string.IsNullOrWhiteSpace(CharacterName):
                _dialogService.ShowError("Введите имя персонажа.");
                return false;
        }

        return true;
    }

    private async Task LoadReferenceDataAsync()
    {
        IsLoading = true;
        try
        {
            var races = await _apiClient.GetRacesAsync();
            Races.Clear();
            foreach (var race in races)
            {
                Races.Add(new SelectionCardItem
                {
                    Id = race.Id, Name = race.Name, Description = race.Description,
                    Subtitle = $"Скорость: {race.BaseSpeed} фт."
                });
            }

            var classes = await _apiClient.GetClassesAsync();
            Classes.Clear();
            foreach (var cls in classes)
            {
                Classes.Add(new SelectionCardItem
                {
                    Id = cls.Id, Name = cls.Name, Description = cls.Description,
                    Subtitle = $"Кость хитов: {cls.HitDie}"
                });
            }

            var backgrounds = await _apiClient.GetBackgroundsAsync();
            Backgrounds.Clear();
            foreach (var bg in backgrounds)
            {
                Backgrounds.Add(new SelectionCardItem
                {
                    Id = bg.Id, Name = bg.Name, Description = bg.Description, Subtitle = bg.Feature
                });
            }

            var traits = await _apiClient.GetTraitsAsync();
            Traits.Clear();
            foreach (var trait in traits)
            {
                Traits.Add(new SelectionCardItem
                {
                    Id = trait.Id, Name = trait.Name, Description = trait.Description, Subtitle = trait.Source
                });
            }

            var spells = await _apiClient.GetSpellsAsync();
            Spells.Clear();
            foreach (var spell in spells)
            {
                Spells.Add(new SelectionCardItem
                {
                    Id = spell.Id, Name = spell.Name, Description = spell.Description,
                    Subtitle = $"Уровень {spell.Level} — {spell.School}"
                });
            }

            if (Races.Count == 0)
                _snackbar.ShowError("Справочники пусты. Запустите API.");
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Ошибка загрузки справочников: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
