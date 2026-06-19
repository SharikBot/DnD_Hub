using System.IO;
using System.Windows;
using DnDCharacterManager.Desktop.Services;
using DnDCharacterManager.Desktop.ViewModels;
using DnDCharacterManager.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DnDCharacterManager.Desktop;

public partial class App : System.Windows.Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "logs", "desktop-.log"),
                rollingInterval: RollingInterval.Day)
            .CreateLogger();

        _host = Host.CreateDefaultBuilder()
            .UseSerilog()
            .ConfigureServices(ConfigureServices)
            .Build();

        await _host.StartAsync();

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        Log.CloseAndFlush();
        base.OnExit(e);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IApiClient, ApiClient>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<MaterialDesignThemes.Wpf.SnackbarMessageQueue>();
        services.AddSingleton<ISnackbarService, SnackbarService>();

        services.AddTransient<MainViewModel>();
        services.AddTransient<CharacterCreatorViewModel>();
        services.AddTransient<CharactersListViewModel>();
        services.AddTransient<BestiaryViewModel>();
        services.AddTransient<RulebookViewModel>();
        services.AddTransient<AiGeneratorViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<CharacterSheetViewModel>();

        services.AddSingleton<MainWindow>();
    }
}
