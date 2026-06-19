using DnDCharacterManager.Core.Common;

namespace DnDCharacterManager.Core.Entities;

public class Inventory : BaseEntity
{
    public Guid CharacterId { get; set; }

    public decimal MaxWeight { get; set; } = 150m;

    public int Gold { get; set; }

    public Character Character { get; set; } = null!;

    public ICollection<InventoryItem> Items { get; set; } = new List<InventoryItem>();
}