using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DnDCharacterManager.Core.DTOs;
using DnDCharacterManager.Desktop.Models;
using DnDCharacterManager.Desktop.Services;

namespace DnDCharacterManager.Desktop.ViewModels;

public partial class AiGeneratorViewModel : ObservableObject
{
    private readonly IApiClient _apiClient;
    private readonly IDialogService _dialogService;
    private readonly ISnackbarService _snackbar;

    public AiGeneratorViewModel(IApiClient apiClient, IDialogService dialogService, ISnackbarService snackbar)
    {
        _apiClient = apiClient;
        _dialogService = dialogService;
        _snackbar = snackbar;
    }

    public IReadOnlyList<AiContentTypeOption> ContentTypeOptions { get; } =
    [
        new() { Value = "backstory", Label = "Биография" },
        new() { Value = "name", Label = "Имя" },
        new() { Value = "personality", Label = "Характер" },
        new() { Value = "build", Label = "Билд" },
        new() { Value = "class-recommendation", Label = "Рекомендация класса" },
        new() { Value = "spell-recommendation", Label = "Рекомендация заклинаний" },
        new() { Value = "npc", Label = "NPC" },
        new() { Value = "encounter", Label = "Столкновение" },
        new() { Value = "quest", Label = "Квест" }
    ];

    [ObservableProperty] private AiContentTypeOption _selectedContentTypeOption = new() { Value = "backstory", Label = "Биография" };
    [ObservableProperty] private string _prompt = string.Empty;
    [ObservableProperty] private string _generatedText = string.Empty;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _statusMessage = string.Empty;
    [ObservableProperty] private int _tokensUsed;

    [RelayCommand(CanExecute = nameof(CanGenerate))]
    private async Task GenerateAsync()
    {
        if (string.IsNullOrWhiteSpace(Prompt))
        {
            _dialogService.ShowError("Введите описание для генерации.");
            return;
        }

        IsLoading = true;
        StatusMessage = "Генерация через Gemini AI...";
        GeneratedText = string.Empty;

        try
        {
            var response = await _apiClient.GenerateAiContentAsync(new AiGenerationRequestDto
            {
                ContentType = SelectedContentTypeOption.Value,
                Prompt = Prompt.Trim(),
                MaxTokens = 1024
            });

            if (response.IsSuccess)
            {
                GeneratedText = response.GeneratedText;
                TokensUsed = response.TokensUsed;
                StatusMessage = "Готово.";
                _snackbar.ShowSuccess("Контент сгенерирован.");
            }
            else
            {
                StatusMessage = response.ErrorMessage ?? "Ошибка генерации.";
                _dialogService.ShowError(StatusMessage);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            _dialogService.ShowError(StatusMessage);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanGenerate() => !IsLoading;
    partial void OnIsLoadingChanged(bool value) => GenerateCommand.NotifyCanExecuteChanged();
}
