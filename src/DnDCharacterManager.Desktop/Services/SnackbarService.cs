using MaterialDesignThemes.Wpf;

namespace DnDCharacterManager.Desktop.Services;

public sealed class SnackbarService : ISnackbarService
{
    private readonly SnackbarMessageQueue _queue;

    public SnackbarService(SnackbarMessageQueue queue)
    {
        _queue = queue;
    }

    public void ShowSuccess(string message) => _queue.Enqueue(message);

    public void ShowInfo(string message) => _queue.Enqueue(message);

    public void ShowError(string message) => _queue.Enqueue($"Ошибка: {message}");
}
