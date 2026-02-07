using Godot;
using System;

public partial class Airborne : MovementState3D
{
    [Export] public MovementState3D walkState;

    public override MovementState3D ProcessPhysics(double delta)
    {
        if (_agent.IsOnFloor())
        {
            var newVel = _agent.Velocity;
            newVel.Y = 0;
            _agent.Velocity = newVel;

            return walkState;
        }

        _agent.Velocity += _agent.GetGravity() * (float)delta;
        return base.ProcessPhysics(delta);
    }
}
