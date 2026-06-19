using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DnDCharacterManager.Core.DTOs;
using DnDCharacterManager.Desktop.Services;

namespace DnDCharacterManager.Desktop.ViewModels;

public partial class CharactersListViewModel : ObservableObject
{
    private readonly IApiClient _apiClient;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    private static readonly Guid DemoUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public CharactersListViewModel(IApiClient apiClient, IDialogService dialogService, INavigationService navigationService)
    {
        _apiClient = apiClient;
        _dialogService = dialogService;
        _navigationService = navigationService;
        _ = LoadCharactersAsync();
    }

    public ObservableCollection<CharacterListItemDto> Characters { get; } = [];

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private CharacterListItemDto? _selectedCharacter;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [RelayCommand]
    private async Task LoadCharactersAsync()
    {
        IsLoading = true;
        try
        {
            var items = await _apiClient.GetCharactersAsync(DemoUserId);
            Characters.Clear();
            foreach (var item in items.Where(c => string.IsNullOrWhiteSpace(SearchText)
                                                  || c.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
            {
                Characters.Add(item);
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Ошибка загрузки персонажей: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void OpenCharacter(CharacterListItemDto? character)
    {
        if (character is null)
        {
            return;
        }

        _navigationService.NavigateTo<CharacterSheetViewModel>(vm => vm.Initialize(character.Id));
    }

    [RelayCommand]
    private async Task DeleteCharacterAsync(CharacterListItemDto? character)
    {
        if (character is null)
        {
            return;
        }

        if (!_dialogService.ShowConfirmation($"Удалить персонажа «{character.Name}»?"))
        {
            return;
        }

        try
        {
            if (await _apiClient.DeleteCharacterAsync(character.Id))
            {
                Characters.Remove(character);
            }
            else
            {
                _dialogService.ShowError("Не удалось удалить персонажа.");
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Ошибка удаления: {ex.Message}");
        }
    }
}
