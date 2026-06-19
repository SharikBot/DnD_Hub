using DnDCharacterManager.Core.Interfaces.Repositories;
using DnDCharacterManager.Core.Interfaces.Services;
using DnDCharacterManager.Infrastructure.Data;
using DnDCharacterManager.Infrastructure.Repositories;
using DnDCharacterManager.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DnDCharacterManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        var provider = configuration["Database:Provider"] ?? "PostgreSQL";

        services.AddDbContext<AppDbContext>(options =>
        {
            if (string.Equals(provider, "Sqlite", StringComparison.OrdinalIgnoreCase))
            {
                options.UseSqlite(connectionString);
            }
            else
            {
                // PostgreSQL: Host=localhost;Port=5432;Database=dnd_character_manager;Username=postgres;Password=...
                options.UseNpgsql(connectionString);
            }
        });

        services.AddScoped<ICharacterRepository, CharacterRepository>();
        services.AddScoped<IMonsterRepository, MonsterRepository>();
        services.AddScoped<IRuleRepository, RuleRepository>();
        services.AddScoped<IReferenceRepository, ReferenceRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddHttpClient<IGeminiAiService, GeminiAiService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(60);
        });

        return services;
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync(cancellationToken);
        await DevDataSeeder.SeedAsync(db, cancellationToken);
    }
}
