using Godot;

// This is an autoload singleton that persists across scene changes
// It holds the multiplayer peer so it doesn't get lost
public partial class NetworkManager : Node
{
  public override void _Ready()
  {
    GD.Print("NetworkManager ready");
  }

  public void CreateServer(int port)
  {
    var peer = new ENetMultiplayerPeer();
    var error = peer.CreateServer(port);
    if (error != Error.Ok)
    {
      GD.PrintErr($"Failed to create server: {error}");
      return;
    }

    Multiplayer.ConnectedToServer += OnConnectOk;
    Multiplayer.ConnectionFailed += OnConnectionFail;

    Multiplayer.MultiplayerPeer = peer;
    GD.Print($"Server created on port {port}");
  }

  public void CreateClient(string address, int port)
  {
    var peer = new ENetMultiplayerPeer();
    var error = peer.CreateClient(address, port);
    if (error != Error.Ok)
    {
      GD.PrintErr($"Failed to create client: {error}");
      return;
    }

    Multiplayer.ConnectedToServer += OnConnectOk;
    Multiplayer.ConnectionFailed += OnConnectionFail;

    Multiplayer.MultiplayerPeer = peer;
    GD.Print($"Client connecting to {address}:{port}");
  }

  private void OnConnectOk()
  {
    GD.Print("Connected to server successfully");
  }

  private void OnConnectionFail()
  {
    GD.PrintErr("Failed to connect to server");
  }
}
