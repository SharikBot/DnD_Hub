using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DnDCharacterManager.Core.DTOs;
using DnDCharacterManager.Core.Enums;
using DnDCharacterManager.Desktop.Helpers;
using DnDCharacterManager.Desktop.Services;

namespace DnDCharacterManager.Desktop.ViewModels;

public partial class CharacterSheetViewModel : ObservableObject
{
    private Guid _characterId;
    private readonly IApiClient _apiClient;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;
    private readonly ISnackbarService _snackbar;

    public CharacterSheetViewModel(
        IApiClient apiClient,
        IDialogService dialogService,
        INavigationService navigationService,
        ISnackbarService snackbar)
    {
        _apiClient = apiClient;
        _dialogService = dialogService;
        _navigationService = navigationService;
        _snackbar = snackbar;
    }

    public ObservableCollection<AbilityScoreDisplay> AbilityScoreDisplays { get; } = [];

    [ObservableProperty] private CharacterDto? _character;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private int _editableCurrentHp;
    [ObservableProperty] private string _editableName = string.Empty;
    [ObservableProperty] private string _editableBackstory = string.Empty;
    [ObservableProperty] private string _pdfPreviewPath = string.Empty;

    public void Initialize(Guid characterId)
    {
        _characterId = characterId;
        _ = LoadCharacterAsync(characterId);
    }

    [RelayCommand]
    private async Task LoadCharacterAsync(Guid characterId)
    {
        IsLoading = true;
        try
        {
            Character = await _apiClient.GetCharacterAsync(characterId);
            if (Character is null)
            {
                _dialogService.ShowError("Персонаж не найден.");
                _navigationService.NavigateTo<CharactersListViewModel>();
                return;
            }

            EditableName = Character.Name;
            EditableCurrentHp = Character.CurrentHitPoints;
            EditableBackstory = Character.Backstory ?? string.Empty;

            AbilityScoreDisplays.Clear();
            foreach (var ability in AbilityHelper.All)
            {
                var score = Character.AbilityScores.GetValueOrDefault(ability, 10);
                AbilityScoreDisplays.Add(new AbilityScoreDisplay
                {
                    Ability = ability,
                    DisplayName = AbilityHelper.GetDisplayName(ability),
                    Score = score,
                    Modifier = AbilityHelper.GetModifier(score)
                });
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Ошибка загрузки: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SaveSheetAsync()
    {
        if (Character is null) return;

        IsLoading = true;
        try
        {
            var updated = await _apiClient.UpdateCharacterSheetAsync(_characterId, new UpdateCharacterSheetDto
            {
                Name = EditableName.Trim(),
                CurrentHitPoints = EditableCurrentHp,
                Backstory = EditableBackstory,
            });

            if (updated is null)
            {
                _dialogService.ShowError("Не удалось сохранить изменения.");
                return;
            }

            Character = updated;
            _snackbar.ShowSuccess("Лист персонажа сохранён.");
        }
        catch (Exception ex)
        {
            _dialogService.ShowError(ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task PreviewPdfAsync()
    {
        if (Character is null) return;

        IsLoading = true;
        try
        {
            var pdf = await _apiClient.DownloadCharacterPdfAsync(_characterId);
            if (pdf is null || pdf.Length == 0)
            {
                _dialogService.ShowError("Не удалось сгенерировать PDF.");
                return;
            }

            var previewDir = Path.Combine(Path.GetTempPath(), "DnDCharacterManager");
            Directory.CreateDirectory(previewDir);
            PdfPreviewPath = Path.Combine(previewDir, $"preview-{Character.Id}.pdf");
            await File.WriteAllBytesAsync(PdfPreviewPath, pdf);
            Process.Start(new ProcessStartInfo(PdfPreviewPath) { UseShellExecute = true });
            _snackbar.ShowInfo("PDF открыт для предпросмотра.");
        }
        catch (Exception ex)
        {
            _dialogService.ShowError(ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DownloadPdfAsync()
    {
        if (Character is null) return;

        var savePath = _dialogService.ShowSaveFileDialog(
            $"{Character.Name}-sheet.pdf",
            "PDF (*.pdf)|*.pdf",
            "Сохранить лист персонажа");

        if (savePath is null) return;

        IsLoading = true;
        try
        {
            var pdf = await _apiClient.DownloadCharacterPdfAsync(_characterId);
            if (pdf is null || pdf.Length == 0)
            {
                _dialogService.ShowError("Не удалось сгенерировать PDF.");
                return;
            }

            await File.WriteAllBytesAsync(savePath, pdf);
            _snackbar.ShowSuccess($"PDF сохранён: {savePath}");
        }
        catch (Exception ex)
        {
            _dialogService.ShowError(ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand] private void AdjustHp(int amount)
    {
        EditableCurrentHp = Math.Max(0, Math.Min(Character?.MaxHitPoints ?? EditableCurrentHp, EditableCurrentHp + amount));
    }

    [RelayCommand] private void GoBack() => _navigationService.NavigateTo<CharactersListViewModel>();

    public string AlignmentDisplay => Character is null ? string.Empty : AlignmentHelper.GetDisplayName(Character.Alignment);
}

public partial class AbilityScoreDisplay : ObservableObject
{
    public AbilityType Ability { get; init; }
    public string DisplayName { get; init; } = string.Empty;

    [ObservableProperty] private int _score;
    [ObservableProperty] private int _modifier;

    partial void OnScoreChanged(int value) => Modifier = AbilityHelper.GetModifier(value);
}