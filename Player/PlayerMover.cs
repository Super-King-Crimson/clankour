using Godot;
using System;

public partial class PlayerMover : Node, IMover
{
    [Export] public CharacterBody3D player;

    public Vector3 GetInputDirection()
    {
        Vector2 input = Input.GetVector("left", "right", "fore", "back");
        Vector3 input3D = new(input.X, 0, input.Y);

        return player.Transform.Basis * input3D;
    }

    public bool WantsJump() => Input.IsActionJustPressed("jump");
}
