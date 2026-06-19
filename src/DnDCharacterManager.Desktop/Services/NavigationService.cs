using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace DnDCharacterManager.Desktop.Services;

public sealed class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ObservableObject? CurrentViewModel { get; private set; }

    public event Action<ObservableObject?>? CurrentViewModelChanged;

    public void NavigateTo<TViewModel>() where TViewModel : ObservableObject
    {
        var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
        SetCurrentViewModel(viewModel);
    }

    public void NavigateTo<TViewModel>(Action<TViewModel> configure) where TViewModel : ObservableObject
    {
        var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
        configure(viewModel);
        SetCurrentViewModel(viewModel);
    }

    private void SetCurrentViewModel(ObservableObject viewModel)
    {
        CurrentViewModel = viewModel;
        CurrentViewModelChanged?.Invoke(CurrentViewModel);
    }
}
