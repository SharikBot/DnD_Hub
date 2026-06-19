using DnDCharacterManager.Core.Common;

namespace DnDCharacterManager.Core.Entities;

public class InventoryItem : BaseEntity
{
    public Guid InventoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Quantity { get; set; } = 1;

    public decimal Weight { get; set; }

    public decimal ValueInGold { get; set; }

    public bool IsEquipped { get; set; }

    public Inventory Inventory { get; set; } = null!;
}
