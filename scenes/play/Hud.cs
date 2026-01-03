using Godot;
using Algonquin1;
using System.Linq;
using System;
using Godot.Collections;

public partial class Hud : CanvasLayer
{
	[Export] public Container InventoryList;
	[Export] public CanvasItem ReadyToFireContainer;
	[Export] public PortUi PortUIContainer;
	[Export] public Label HealthLabel;
	[Export] public Node3D PlayersContainer;

	private Player _player;
	private int _retryCount = 0;
	private const int MaxRetries = 30; // Try for ~1 second

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

		FindLocalPlayer();
	}

	private void OnPlayerEnteredPort(Port port, Player player, Variant payload)
	{
		GD.Print($"Player {player.Name} entered port {port.PortName}");
		if (player.Name == _player.Name)
		{
			PortUIContainer.Visible = true;
			var payloadDict = (Dictionary)payload;
			PortUIContainer.ChangeName((string)payloadDict["PortName"]);
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
		// Find the player that we control
		var myPeerId = Multiplayer.GetUniqueId();
		_player = PlayersContainer.GetNodeOrNull<Player>($"player_{myPeerId}");

		if (_player != null)
		{
			_player.InventoryChanged += OnInventoryChanged;

			_player.CannonReadyToFire += () =>
			{
				ReadyToFireContainer.Visible = true;
			};

			_player.CannonFired += () =>
			{
				ReadyToFireContainer.Visible = false;
			};

			_player.HealthUpdate += (newHealth) =>
			{
				HealthLabel.Text = $"Health: {newHealth}";
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
}
