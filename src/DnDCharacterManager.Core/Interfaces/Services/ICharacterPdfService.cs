namespace DnDCharacterManager.Core.Interfaces.Services;

public interface ICharacterPdfService
{
    Task<byte[]> GenerateCharacterSheetAsync(Guid characterId, CancellationToken cancellationToken = default);
}
