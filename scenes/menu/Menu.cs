using Godot;
using System;
using PiratesQuest;

public partial class Menu : Node2D
{
  [Export] public Container MultiplayerControls;
  [Export] public Container ServerListingsContainer;
  [Export] public Container PlayerIdentityContainer;

  private PackedScene _listingScene = GD.Load<PackedScene>("res://scenes/menu/scenes/server_listing.tscn");

  public override void _Ready()
  {
    if (Configuration.IsDesignatedServerMode())
    {
      GD.Print($"Starting server on port {Configuration.DefaultPort} due to --server flag");
      CallDeferred(MethodName.StartServer);
    }
    else
    {
      SetupMenuUI();
    }
  }

  private void SetupMenuUI()
  {
    // Player Identity Handlers
    PlayerIdentityContainer.GetNode<LineEdit>("PlayerNameEdit").TextChanged += (newText) =>
    {
      var identity = GetNode<Identity>("/root/Identity");
      identity.PlayerName = newText;
    };

    // Connect to Server Listings
    foreach (var serverListing in Configuration.GetDefaultServerListings())
    {
      var listingInstance = _listingScene.Instantiate<ServerListing>();
      listingInstance.ServerName = serverListing.ServerName;
      listingInstance.IpAddress = serverListing.IpAddress;
      listingInstance.Port = serverListing.Port;
      listingInstance.PlayerCount = "x";
      listingInstance.PlayerMax = "8";

      ServerListingsContainer.AddChild(listingInstance);

      listingInstance.JoinServer += (ip, port) =>
      {
        JoinServer(ip, port);
      };
    }

    // Custom join 
    var joinButton = MultiplayerControls.GetNodeOrNull<Button>("JoinButton");
    joinButton.ButtonDown += () =>
    {
      var ipBox = MultiplayerControls.GetNodeOrNull<LineEdit>("ServerIP");
      var ip = ipBox.Text.Trim();
      JoinServer(ip, Configuration.DefaultPort);
    };

    // Version Label
    var versionLabel = GetNodeOrNull<Label>("CanvasLayer/VersionLabel");
    if (versionLabel != null)
    {
      versionLabel.Text = Configuration.GetVersion();
    }
  }

  private void JoinServer(string ipAddress, int port)
  {
    var networkManager = GetNode<NetworkManager>("/root/NetworkManager");
    networkManager.CreateClient(ipAddress, port);
    GetTree().ChangeSceneToFile("res://scenes/play/play.tscn");
  }

  private void StartServer()
  {
    var networkManager = GetNode<NetworkManager>("/root/NetworkManager");
    networkManager.CreateServer(Configuration.DefaultPort);
    GetTree().ChangeSceneToFile("res://scenes/play/play.tscn");
  }
}
