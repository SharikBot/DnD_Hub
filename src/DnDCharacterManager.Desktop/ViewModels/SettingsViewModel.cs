using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DnDCharacterManager.Core.Patterns.Singleton;
using DnDCharacterManager.Desktop.Services;

namespace DnDCharacterManager.Desktop.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IApiClient _apiClient;
    private readonly IDialogService _dialogService;

    public SettingsViewModel(IApiClient apiClient, IDialogService dialogService)
    {
        _apiClient = apiClient;
        _dialogService = dialogService;

        var config = AppConfiguration.Instance;
        ApiBaseUrl = config.ApiBaseUrl;
        GeminiApiKey = config.GeminiApiKey;
    }

    [ObservableProperty]
    private string _apiBaseUrl = string.Empty;

    [ObservableProperty]
    private string _geminiApiKey = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [RelayCommand]
    private void Save()
    {
        var config = AppConfiguration.Instance;
        config.ApiBaseUrl = ApiBaseUrl.Trim();
        config.GeminiApiKey = GeminiApiKey.Trim();
        _apiClient.BaseUrl = config.ApiBaseUrl;

        StatusMessage = "Настройки сохранены.";
        _dialogService.ShowInfo("Настройки успешно сохранены.");
    }

    [RelayCommand]
    private void ResetToDefaults()
    {
        AppConfiguration.Reset();
        var config = AppConfiguration.Instance;
        ApiBaseUrl = config.ApiBaseUrl;
        GeminiApiKey = config.GeminiApiKey;
        _apiClient.BaseUrl = config.ApiBaseUrl;
        StatusMessage = "Восстановлены значения по умолчанию.";
    }
}
