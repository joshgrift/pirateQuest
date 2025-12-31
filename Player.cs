using Godot;

public partial class Player : CharacterBody3D
{
  // Don't forget to rebuild the project so the editor knows about the new export variable.

  // How fast the player moves in meters per second.
  [Export]
  public int Speed { get; set; } = 14;
  // The downward acceleration when in the air, in meters per second squared.
  [Export]
  public int FallAcceleration { get; set; } = 75;
  // How fast the player rotates (higher = faster rotation)
  [Export]
  public float RotationSpeed { get; set; } = 1.0f;

  private Vector3 _targetVelocity = Vector3.Zero;

  public override void _PhysicsProcess(double delta)
  {
    // We create a local variable to store the input direction.
    var direction = Vector3.Zero;

    // We check for each move input and update the direction accordingly.
    if (Input.IsActionPressed("move_right"))
    {
      direction.X += 1.0f;
    }
    if (Input.IsActionPressed("move_left"))
    {
      direction.X -= 1.0f;
    }
    if (Input.IsActionPressed("move_back"))
    {
      // Notice how we are working with the vector's X and Z axes.
      // In 3D, the XZ plane is the ground plane.
      direction.Z += 1.0f;
    }
    if (Input.IsActionPressed("move_forward"))
    {
      direction.Z -= 1.0f;
    }

    if (direction != Vector3.Zero)
    {
      direction = direction.Normalized();
      // Smoothly interpolate the rotation using Slerp
      var pivot = GetNode<Node3D>("Pivot");
      var targetBasis = Basis.LookingAt(direction, Vector3.Up).Orthonormalized();
      var currentBasis = pivot.Basis.Orthonormalized();
      pivot.Basis = currentBasis.Slerp(targetBasis, RotationSpeed * (float)delta);
    }

    // Ground velocity
    _targetVelocity.X = direction.X * Speed;
    _targetVelocity.Z = direction.Z * Speed;

    // Vertical velocity
    if (!IsOnFloor()) // If in the air, fall towards the floor. Literally gravity
    {
      _targetVelocity.Y -= FallAcceleration * (float)delta;
    }

    // Moving the character
    Velocity = _targetVelocity;
    MoveAndSlide();
  }
}
