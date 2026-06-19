using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DnDCharacterManager.Desktop.Models;
using DnDCharacterManager.Desktop.Services;
using MaterialDesignThemes.Wpf;

namespace DnDCharacterManager.Desktop.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;

    public MainViewModel(INavigationService navigationService, SnackbarMessageQueue messageQueue)
    {
        _navigationService = navigationService;
        MessageQueue = messageQueue;
        _navigationService.CurrentViewModelChanged += OnNavigationViewModelChanged;

        MenuItems =
        [
            new NavigationMenuItem { Title = "Создать персонажа", Icon = PackIconKind.AccountPlus, ViewModelType = typeof(CharacterCreatorViewModel) },
            new NavigationMenuItem { Title = "Созданные персонажи", Icon = PackIconKind.AccountGroup, ViewModelType = typeof(CharactersListViewModel) },
            new NavigationMenuItem { Title = "Бестиарий", Icon = PackIconKind.BookOpenVariant, ViewModelType = typeof(BestiaryViewModel) },
            new NavigationMenuItem { Title = "Правила", Icon = PackIconKind.BookAlphabet, ViewModelType = typeof(RulebookViewModel) },
            new NavigationMenuItem { Title = "Генератор AI", Icon = PackIconKind.Robot, ViewModelType = typeof(AiGeneratorViewModel) },
            new NavigationMenuItem { Title = "Настройки", Icon = PackIconKind.Cog, ViewModelType = typeof(SettingsViewModel) }
        ];

        NavigateToMenuItem(MenuItems[0]);
    }

    public ObservableCollection<NavigationMenuItem> MenuItems { get; }

    public SnackbarMessageQueue MessageQueue { get; }

    [ObservableProperty]
    private ObservableObject? _currentViewModel;

    [ObservableProperty]
    private string _windowTitle = "DnD Character Manager";

    [RelayCommand]
    private void Navigate(NavigationMenuItem? item)
    {
        if (item is null)
        {
            return;
        }

        NavigateToMenuItem(item);
    }

    private void NavigateToMenuItem(NavigationMenuItem item)
    {
        foreach (var menuItem in MenuItems)
        {
            menuItem.IsSelected = menuItem == item;
        }

        WindowTitle = $"DnD Character Manager — {item.Title}";

        switch (item.ViewModelType.Name)
        {
            case nameof(CharacterCreatorViewModel):
                _navigationService.NavigateTo<CharacterCreatorViewModel>();
                break;
            case nameof(CharactersListViewModel):
                _navigationService.NavigateTo<CharactersListViewModel>();
                break;
            case nameof(BestiaryViewModel):
                _navigationService.NavigateTo<BestiaryViewModel>();
                break;
            case nameof(RulebookViewModel):
                _navigationService.NavigateTo<RulebookViewModel>();
                break;
            case nameof(AiGeneratorViewModel):
                _navigationService.NavigateTo<AiGeneratorViewModel>();
                break;
            case nameof(SettingsViewModel):
                _navigationService.NavigateTo<SettingsViewModel>();
                break;
        }
    }

    private void OnNavigationViewModelChanged(ObservableObject? viewModel)
    {
        CurrentViewModel = viewModel;
    }
}