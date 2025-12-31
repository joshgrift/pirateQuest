using Godot;
using System;

public partial class Menu : Node2D
{
	[Export] public Container MultiplayerControls;
	[Export] public Button StartButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;

		var hostButton = MultiplayerControls.GetNodeOrNull<Button>("HostButton");
		hostButton.ButtonDown += () =>
		{
			var networkManager = GetNode<NetworkManager>("/root/NetworkManager");
			networkManager.CreateServer(7777);
		};

		var joinButton = MultiplayerControls.GetNodeOrNull<Button>("JoinButton");
		joinButton.ButtonDown += () =>
		{
			var ipBox = MultiplayerControls.GetNodeOrNull<LineEdit>("ServerIP");
			var ip = ipBox.Text.Trim();
			var networkManager = GetNode<NetworkManager>("/root/NetworkManager");
			networkManager.CreateClient(ip, 7777);
		};

		StartButton.ButtonDown += () =>
		{
			Rpc(MethodName.StartGame);
		};

	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void StartGame()
	{
		GetTree().ChangeSceneToFile("res://scenes/play/play.tscn");
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.F)
		{
			GD.Print("Toggling mouse mode");
			Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured
				? Input.MouseModeEnum.Visible
				: Input.MouseModeEnum.Captured;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
