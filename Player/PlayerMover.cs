using Godot;
using System;

public partial class PlayerMover : Node, IMover
{
    public Vector2 GetInputDirection() => Input.GetVector("left", "right", "fore", "back");
    public bool WantsJump() => Input.IsActionPressed("jump");
}
