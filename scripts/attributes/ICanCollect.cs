namespace Algonquin1.Attributes;

/// <summary>
/// Interface for objects that can collect items from a Dropper.
/// </summary>
public interface ICanCollect
{
  bool UpdateInventory(InventoryItemType item, int amount);
}