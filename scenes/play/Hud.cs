using Godot;
using Algonquin1;

public partial class Hud : CanvasLayer
{
	[Export] public Container InventoryList;

	private Player _player;
	private int _retryCount = 0;
	private const int MaxRetries = 30; // Try for ~1 second

	public override void _Ready()
	{
		// Start looking for the player
		FindLocalPlayer();
	}

	private void FindLocalPlayer()
	{
		// Find the player that we control
		var myPeerId = Multiplayer.GetUniqueId();
		_player = GetNodeOrNull<Player>($"/root/Play/SpawnPoint/Player{myPeerId}");

		if (_player != null)
		{
			_player.InventoryChanged += OnInventoryChanged;
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

	private void OnInventoryChanged(InventoryItemType itemType, int newAmount)
	{
		var itemLabel = InventoryList.GetNodeOrNull<Label>($"{itemType}Label");

		if (itemLabel != null)
		{
			itemLabel.Text = $"{itemType}: {newAmount}";
		}
		else
		{
			itemLabel = new Label
			{
				Name = $"{itemType}Label",
				Text = $"{itemType}: {newAmount}"
			};
			InventoryList.AddChild(itemLabel);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
