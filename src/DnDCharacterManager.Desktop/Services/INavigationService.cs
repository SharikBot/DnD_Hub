using CommunityToolkit.Mvvm.ComponentModel;

namespace DnDCharacterManager.Desktop.Services;

public interface INavigationService
{
    ObservableObject? CurrentViewModel { get; }

    event Action<ObservableObject?>? CurrentViewModelChanged;

    void NavigateTo<TViewModel>() where TViewModel : ObservableObject;

    void NavigateTo<TViewModel>(Action<TViewModel> configure) where TViewModel : ObservableObject;
}
