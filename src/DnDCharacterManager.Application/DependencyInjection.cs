using DnDCharacterManager.Application.Services;
using DnDCharacterManager.Application.Validators;
using DnDCharacterManager.Core.Interfaces.Services;
using DnDCharacterManager.Core.Patterns.Factory;
using Microsoft.Extensions.DependencyInjection;

namespace DnDCharacterManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMemoryCache();

        services.AddScoped<ICharacterFactory, CharacterFactory>();
        services.AddScoped<CreateCharacterValidator>();
        services.AddScoped<ICharacterService, CharacterService>();
        services.AddScoped<IMonsterService, MonsterService>();
        services.AddScoped<IRuleService, RuleService>();
        services.AddScoped<IReferenceService, ReferenceService>();
        services.AddScoped<ICharacterPdfService, CharacterPdfService>();

        return services;
    }
}
