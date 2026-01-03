using Godot;

public partial class CameraPivot : Marker3D
{
	[Export]
	public float MouseSensitivity { get; set; } = 0.002f;

	// Clamp vertical rotation to prevent camera from going below the world
	// 0 = looking straight ahead (horizon)
	// Negative values = looking up, Positive values = looking down
	[Export]
	public float MinPitch { get; set; } = -Mathf.Pi / 2.0f; // Looking straight up

	[Export]
	public float MaxPitch { get; set; } = 0.3f; // Can look down a bit, but not too far below the world

	private float _cameraTargetAngleY = 0.0f; // Yaw (horizontal rotation)
	private float _cameraTargetAngleX = 0.0f; // Pitch (vertical rotation)
	private bool _isDragging = false;

	public override void _Input(InputEvent @event)
	{
		// Start dragging when left mouse button is pressed
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left)
			{
				_isDragging = mouseButton.Pressed;
			}
		}

		// Only rotate the camera when dragging
		if (@event is InputEventMouseMotion mouseMotion && _isDragging)
		{
			// Horizontal rotation (yaw) - rotate around Y axis
			_cameraTargetAngleY -= mouseMotion.Relative.X * MouseSensitivity;

			// Vertical rotation (pitch) - rotate around X axis
			_cameraTargetAngleX -= mouseMotion.Relative.Y * MouseSensitivity;

			// Clamp the vertical rotation to prevent camera from going below the world
			_cameraTargetAngleX = Mathf.Clamp(_cameraTargetAngleX, MinPitch, MaxPitch);

			// Apply the rotation (X = pitch, Y = yaw, Z = roll)
			Rotation = new Vector3(_cameraTargetAngleX, _cameraTargetAngleY, Rotation.Z);
		}
	}

	// If the pivot needs to follow a moving target (like a player), update its position in _process or _physics_process
	// public override void _Process(double delta)
	// {
	//     GlobalPosition = GetParent<Node3D>().GlobalPosition;
	// }
}
