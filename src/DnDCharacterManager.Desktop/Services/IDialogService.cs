namespace DnDCharacterManager.Desktop.Services;

public interface IDialogService
{
    void ShowInfo(string message, string title = "Информация");
    void ShowError(string message, string title = "Ошибка");
    bool ShowConfirmation(string message, string title = "Подтверждение");
    string? ShowOpenFileDialog(string filter, string title = "Выберите файл");
    string? ShowSaveFileDialog(string defaultFileName, string filter, string title = "Сохранить файл");
}
