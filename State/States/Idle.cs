using Godot;
using System;

public partial class Idle : GroundedState3D
{
    [Export] public GroundedState3D walkState;
    [Export] public GroundedState3D runState;

    protected override MovementState3D GetGroundedState()
    {
        if (this.GetInputDirection() != Vector3.Zero)
            return _agent.Velocity.Length() > GroundedState3D.RunSpeed
                ? runState
                : walkState;

        return null;
    }

    public override MovementState3D ProcessPhysics(double delta)
    {
        if (this.GetAerialState() is MovementState3D aerialState)
            return aerialState;

        if (this.GetGroundedState() is MovementState3D groundedState)
            return groundedState;

        if (GetGroundedState() is MovementState3D)
            return this.walkState;

        _agent.Velocity = Vector3.Zero;

        return base.ProcessPhysics(delta);
    }
}
