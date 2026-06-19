using CommunityToolkit.Mvvm.ComponentModel;
using MaterialDesignThemes.Wpf;

namespace DnDCharacterManager.Desktop.Models;

public partial class NavigationMenuItem : ObservableObject
{
    public string Title { get; init; } = string.Empty;

    public PackIconKind Icon { get; init; }

    public Type ViewModelType { get; init; } = typeof(object);

    [ObservableProperty]
    private bool _isSelected;
}
