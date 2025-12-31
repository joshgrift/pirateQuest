
namespace Algonquin1;

using System.Collections.Generic;

public partial class Inventory
{
  private readonly Dictionary<InventoryItemType, int> _items = [];

  public int GetItemCount(InventoryItemType itemType)
  {
    if (_items.TryGetValue(itemType, out int count))
    {
      return count;
    }
    else
    {
      return 0;
    }

  }

  public void AddItem(InventoryItemType itemType, int amount)
  {
    if (_items.ContainsKey(itemType))
    {
      _items[itemType] += amount;
    }
    else
    {
      _items[itemType] = amount;
    }
  }

  public bool RemoveItem(InventoryItemType itemType, int amount)
  {
    var foundItem = _items.TryGetValue(itemType, out int count);
    if (foundItem && count >= amount)
    {
      _items[itemType] -= amount;
      return true;
    }

    return false;
  }
}