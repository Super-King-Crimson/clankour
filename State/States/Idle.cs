using Godot;
using System;

public partial class Idle : MovementState3D
{
    [Export] public MovementState3D walkState;
    [Export] public MovementState3D jumpState;

    public override MovementState3D ProcessPhysics(double delta)
    {
        if (this.GetInputDirection() != Vector2.Zero)
            return walkState;

        if (this.WantsJump())
            return jumpState;

        _agent.Velocity = Vector3.Zero;

        return base.ProcessPhysics(delta);
    }
}
