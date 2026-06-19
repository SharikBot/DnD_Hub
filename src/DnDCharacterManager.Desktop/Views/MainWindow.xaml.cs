using System.Windows;
using DnDCharacterManager.Desktop.ViewModels;

namespace DnDCharacterManager.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
