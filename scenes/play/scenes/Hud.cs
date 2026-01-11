using Godot;
using PiratesQuest;
using System.Linq;
using PiratesQuest.Data;
using Godot.Collections;

public partial class Hud : Control
{
  [Export] public Tree InventoryList;
  [Export] public PortUi PortUIContainer;
  [Export] public Label HealthLabel;
  [Export] public Node3D PlayersContainer;
  [Export] public Tree LeaderboardTree;

  [Export] public VBoxContainer StatusListContainer;

  private Player _player;
  private int _retryCount = 0;
  private const int MaxRetries = 30;
  private Dictionary<InventoryItemType, TreeItem> InventoryTreeReferences = [];
  private TreeItem rootInventoryItem = null;

  public override void _Ready()
  {
    PortUIContainer.Visible = false;
    if (Multiplayer.IsServer())
    {
      GD.Print("Skipping HUD, acting as server");
      QueueFree();
      return;
    }

    var ports = GetTree().GetNodesInGroup("ports");

    GD.Print($"HUD found {ports.Count} ports in the scene");

    foreach (Port port in ports.Cast<Port>())
    {
      GD.Print($"HUD subscribing to port {port.PortName} events");
      port.ShipDocked += OnPlayerEnteredPort;
      port.ShipDeparted += OnPlayerDepartedPort;
    }

    CallDeferred(MethodName.FindLocalPlayer);

    var emptyStylebox = new StyleBoxEmpty();
    LeaderboardTree.AddThemeStyleboxOverride("panel", emptyStylebox);
    LeaderboardTree.AddThemeStyleboxOverride("bg", emptyStylebox);
    LeaderboardTree.AddThemeConstantOverride("draw_relationship_lines", 0);
    LeaderboardTree.AddThemeConstantOverride("draw_guides", 0);
    LeaderboardTree.AddThemeConstantOverride("v_separation", 0);
    LeaderboardTree.MouseFilter = Control.MouseFilterEnum.Ignore;
    LeaderboardTree.HideRoot = true;

    UpdateLeaderboard();
  }

  private void OnPlayerEnteredPort(Port port, Player player, Variant payload)
  {
    GD.Print($"Player {player.Name} entered port {port.PortName}");
    if (player.Name == _player.Name)
    {
      PortUIContainer.Player = _player;
      PortUIContainer.Visible = true;
      var payloadDict = (Dictionary)payload;
      PortUIContainer.ChangeName((string)payloadDict["PortName"]);

      // Convert Godot Array to ShopItemData[]
      var godotArray = payloadDict["ItemsForSale"].AsGodotArray();
      var shopItems = new ShopItemData[godotArray.Count];
      for (int i = 0; i < godotArray.Count; i++)
      {
        shopItems[i] = (ShopItemData)godotArray[i];
      }

      GD.Print($"Setting port UI stock with {shopItems.Length} items");
      PortUIContainer.SetStock(shopItems);
      PortUIContainer.UpdateShipMenu();
    }
  }

  private void OnPlayerDepartedPort(Port port, Player player)
  {
    if (player.Name == _player.Name)
    {
      PortUIContainer.Visible = false;
    }
  }

  private void FindLocalPlayer()
  {
    if (PlayersContainer == null)
    {
      GD.PrintErr("PlayersContainer is not set in HUD");
      return;
    }

    // Find the player that we control
    var myPeerId = Multiplayer.GetUniqueId();
    GD.Print($"{PlayersContainer.GetChildCount()}");
    _player = PlayersContainer.GetNodeOrNull<Player>($"player_{myPeerId}");

    if (_player != null)
    {
      _player.InventoryChanged += OnInventoryChanged;
      InitializeInventory();

      _player.CannonReadyToFire += () =>
      {
        StatusListContainer.GetNode<Control>("ReadyToFire").Visible = true;
      };

      _player.CannonFired += () =>
      {
        StatusListContainer.GetNode<Control>("ReadyToFire").Visible = false;
      };

      _player.HealthUpdate += (newHealth) =>
      {
        HealthLabel.Text = $"{newHealth}";
      };

      GD.Print($"HUD connected to Player{myPeerId}");
    }
    else
    {
      _retryCount++;
      if (_retryCount < MaxRetries)
      {
        // Retry in the next frame
        GetTree().CreateTimer(0.033f).Timeout += FindLocalPlayer;
      }
      else
      {
        GD.PrintErr($"Could not find Player{myPeerId} after {MaxRetries} attempts");
      }
    }
  }

  private void UpdateLeaderboard()
  {
    LeaderboardTree.Clear();
    LeaderboardTree.Columns = 2;
    LeaderboardTree.SetColumnCustomMinimumWidth(0, 32);
    LeaderboardTree.SetColumnCustomMinimumWidth(1, 100);

    var rootItem = LeaderboardTree.CreateItem();

    var players = PlayersContainer.GetChildren().OfType<Player>()
        .OrderByDescending(p => p.GetInventoryCount(InventoryItemType.Trophy));

    foreach (var player in players)
    {
      if (player.Nickname == "" || player.Nickname == null)
        continue;
      var item = LeaderboardTree.CreateItem(rootItem);
      item.SetText(0, player.GetInventoryCount(InventoryItemType.Trophy).ToString());
      item.SetIcon(0, Icons.GetInventoryIcon(InventoryItemType.Trophy));
      item.SetText(1, player.Nickname);
    }

    GetTree().CreateTimer(5.0f).Timeout += UpdateLeaderboard;
  }

  private void InitializeInventory()
  {
    rootInventoryItem = InventoryList.CreateItem();

    InventoryList.Columns = 2;
    InventoryList.MouseFilter = Control.MouseFilterEnum.Ignore;
    InventoryList.HideRoot = true; // Hide the root item and its line

    InventoryList.SetColumnCustomMinimumWidth(0, 32);
    InventoryList.SetColumnCustomMinimumWidth(1, 100);
    InventoryList.CustomMinimumSize = new Vector2(152, 0);
    InventoryList.SizeFlagsHorizontal = Control.SizeFlags.ShrinkBegin;

    InventoryList.AddThemeConstantOverride("draw_relationship_lines", 0);
    InventoryList.AddThemeConstantOverride("draw_guides", 0);
    InventoryList.AddThemeConstantOverride("v_separation", 0); // Remove vertical spacing between items

    var emptyStylebox = new StyleBoxEmpty();
    InventoryList.AddThemeStyleboxOverride("panel", emptyStylebox);
    InventoryList.AddThemeStyleboxOverride("bg", emptyStylebox);

    var inventory = _player.GetInventory();
    foreach (var kvp in inventory)
    {
      OnInventoryChanged(kvp.Key, kvp.Value);
    }
  }

  private void OnInventoryChanged(InventoryItemType itemType, int newAmount)
  {
    if (_player.isLimitedByCapacity)
    {
      StatusListContainer.GetNode<Control>("Heavy").Visible = true;
    }
    else
    {
      StatusListContainer.GetNode<Control>("Heavy").Visible = false;
    }

    if (InventoryTreeReferences.TryGetValue(itemType, out TreeItem itemEntry))
    {
      itemEntry.SetText(1, newAmount.ToString());
      return;
    }
    else
    {
      TreeItem item = InventoryList.CreateItem(rootInventoryItem);
      item.SetIcon(0, Icons.GetInventoryIcon(itemType));
      item.SetText(1, newAmount.ToString());
      InventoryTreeReferences.Add(itemType, item);
    }
  }
}
