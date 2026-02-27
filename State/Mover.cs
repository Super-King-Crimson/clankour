using Godot;

public abstract partial class Mover : Node
{
    public abstract Vector2 GetInputDirection();
    public abstract bool WantsJump();

    public abstract Vector3 GetInputDirectionRelative(Transform3D transform);
}
