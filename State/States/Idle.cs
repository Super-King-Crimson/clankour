using Godot;
using System;

public partial class Idle : GroundedState3D
{
    [Export] public Walking walkState;

    public override MovementState3D ProcessPhysics(double delta)
    {
        if (this.GetAerialState(delta) is MovementState3D aerialState)
            return aerialState;

        Vector2 inputDir = this.GetInputDirection();
        if (inputDir != Vector2.Zero)
            return this.walkState;

        _agent.Velocity = Vector3.Zero;

        return base.ProcessPhysics(delta);
    }
}
