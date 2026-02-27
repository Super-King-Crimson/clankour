using Godot;
using System;

public partial class PlayerMover : Mover
{
    public override Vector2 GetInputDirection() => Input.GetVector("left", "right", "fore", "back");

    public override Vector3 GetInputDirectionRelative(Transform3D transform)
    {
        Vector2 input = this.GetInputDirection();
        Vector3 input3D = new(input.X, 0, input.Y);

        return transform.Basis * input3D;
    }

    public override bool WantsJump() => Input.IsActionPressed("jump");
}
