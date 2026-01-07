using Godot;
using System;
using PiratesQuest;

public partial class Menu : Node2D
{
	[Export] public Container MultiplayerControls;

	private int _port = 8888;

	public override void _Ready()
	{
		if (Configuration.IsDesignatedServerMode())
		{
			GD.Print($"Starting server on port {_port} due to --server flag");
			CallDeferred(MethodName.StartServer);

		}
		else
		{
			SetupMenuUI();
		}
	}

	private void SetupMenuUI()
	{
		var joinButton = MultiplayerControls.GetNodeOrNull<Button>("JoinButton");
		joinButton.ButtonDown += () =>
		{
			var ipBox = MultiplayerControls.GetNodeOrNull<LineEdit>("ServerIP");
			var ip = ipBox.Text.Trim();

			// Connect to server FIRST (NetworkManager is persistent)
			var networkManager = GetNode<NetworkManager>("/root/NetworkManager");
			networkManager.CreateClient(ip, _port);

			// THEN change scene
			GetTree().ChangeSceneToFile("res://scenes/play/play.tscn");
		};

		var versionLabel = GetNodeOrNull<Label>("CanvasLayer/VersionLabel");
		if (versionLabel != null)
		{
			versionLabel.Text = Configuration.GetVersion();
		}
	}

	private void StartServer()
	{
		var networkManager = GetNode<NetworkManager>("/root/NetworkManager");
		networkManager.CreateServer(_port);
		GetTree().ChangeSceneToFile("res://scenes/play/play.tscn");
	}
}
