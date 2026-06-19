using CommunityToolkit.Mvvm.ComponentModel;

namespace DnDCharacterManager.Desktop.Models;

public partial class SelectionCardItem : ObservableObject
{
    public Guid Id { get; init; }

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _subtitle = string.Empty;

    [ObservableProperty]
    private bool _isSelected;
}