namespace DnDCharacterManager.Core.Patterns.Singleton;

public sealed class AppConfiguration
{
    private static readonly Lock SyncRoot = new();
    private static AppConfiguration? _instance;

    private AppConfiguration()
    {
        ApiBaseUrl = "http://localhost:5049";
        GeminiApiKey = string.Empty;
    }

    public static AppConfiguration Instance
    {
        get
        {
            if (_instance is not null)
            {
                return _instance;
            }

            lock (SyncRoot)
            {
                _instance ??= new AppConfiguration();
            }

            return _instance;
        }
    }

    public string ApiBaseUrl { get; set; }

    public string GeminiApiKey { get; set; }

    public static void Reset(string? apiBaseUrl = null, string? geminiApiKey = null)
    {
        lock (SyncRoot)
        {
            _instance = new AppConfiguration();

            if (apiBaseUrl is not null)
            {
                _instance.ApiBaseUrl = apiBaseUrl;
            }

            if (geminiApiKey is not null)
            {
                _instance.GeminiApiKey = geminiApiKey;
            }
        }
    }
}
