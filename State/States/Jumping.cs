using Godot;
using System;

public partial class Jumping : MovementState3D
{
    [Export] public MovementState3D airborneState;

    [Export] public float jumpVelocity = 4.5f;

    public override MovementState3D ProcessPhysics(float delta)
    {
        var newVel = _agent.Velocity;
        newVel.Y = jumpVelocity;

        _agent.Velocity = newVel;
        base.ProcessPhysics(delta);

        return airborneState;
    }
}
