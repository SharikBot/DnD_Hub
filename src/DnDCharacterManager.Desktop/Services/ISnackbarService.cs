namespace DnDCharacterManager.Desktop.Services;

public interface ISnackbarService
{
    void ShowSuccess(string message);
    void ShowInfo(string message);
    void ShowError(string message);
}
