namespace PiratesQuest;

using Godot;
using PiratesQuest.Data;
using Godot.Collections;
using PiratesQuest.Attributes;

public partial class DeadPlayer : Area3D
{

  [Export] public Dictionary<InventoryItemType, int> DroppedItems = [];
  [Export] public string Nickname = "";

  public override void _Ready()
  {
    BodyEntered += OnBodyEntered;
  }

  private void OnBodyEntered(Node3D body)
  {
    if (body is ICanCollect collector)
    {
      collector.BulkCollect(DroppedItems);
      QueueFree();
    }
  }
}
