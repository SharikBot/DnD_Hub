using System.Diagnostics;
using System.Net.Http;

var installDir = AppContext.BaseDirectory;
var apiDir = Path.Combine(installDir, "Api");
var desktopDir = Path.Combine(installDir, "Desktop");
var apiExe = Path.Combine(apiDir, "DnDCharacterManager.Api.exe");
var desktopExe = Path.Combine(desktopDir, "DnDCharacterManager.Desktop.exe");

if (!File.Exists(apiExe) || !File.Exists(desktopExe))
{
    MessageBox(
        "Не найдены файлы приложения.\nПереустановите DnD Character Manager.",
        "Ошибка запуска");
    return 1;
}

Process? apiProcess = null;
try
{
    apiProcess = Process.Start(new ProcessStartInfo
    {
        FileName = apiExe,
        WorkingDirectory = apiDir,
        UseShellExecute = false,
        CreateNoWindow = true,
        WindowStyle = ProcessWindowStyle.Hidden,
        Environment =
        {
            ["ASPNETCORE_ENVIRONMENT"] = "Production",
            ["ASPNETCORE_URLS"] = "http://localhost:5049"
        }
    });

    if (apiProcess is null || !await WaitForApiAsync(TimeSpan.FromSeconds(45)))
    {
        MessageBox(
            "Не удалось запустить локальный API на порту 5049.\nПроверьте, не занят ли порт другим приложением.",
            "DnD Character Manager");
        return 1;
    }

    using var desktop = Process.Start(new ProcessStartInfo
    {
        FileName = desktopExe,
        WorkingDirectory = desktopDir,
        UseShellExecute = true
    });

    if (desktop is null)
    {
        MessageBox("Не удалось запустить интерфейс приложения.", "DnD Character Manager");
        return 1;
    }

    await desktop.WaitForExitAsync();
    return desktop.ExitCode;
}
finally
{
    if (apiProcess is { HasExited: false })
    {
        try
        {
            apiProcess.Kill(entireProcessTree: true);
        }
        catch
        {
            // ignored on shutdown
        }
    }
}

static async Task<bool> WaitForApiAsync(TimeSpan timeout)
{
    using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
    var deadline = DateTime.UtcNow + timeout;

    while (DateTime.UtcNow < deadline)
    {
        try
        {
            using var response = await client.GetAsync("http://localhost:5049/api/reference/races");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
        }
        catch
        {
            // API ещё поднимается
        }

        await Task.Delay(500);
    }

    return false;
}

static void MessageBox(string text, string caption)
{
    NativeMethods.MessageBox(IntPtr.Zero, text, caption, 0x10);
}

internal static class NativeMethods
{
    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    internal static extern int MessageBox(IntPtr hWnd, string text, string caption, int type);
}
