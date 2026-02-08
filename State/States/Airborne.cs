using Godot;
using System;

public partial class Airborne : MovementState3D
{
    [Export] public GroundedMovementState3D walkState;
    [Export] public MovementState3D idleState;

    private void BaseEnterMethod(StringName _) => base.Enter(null);
    private bool _connected = false;

    public override MovementState3D Enter(State prevState)
    {
        if (prevState is Jumping)
        {
            _connected = true;
            _animator.AnimationFinished += BaseEnterMethod;

            return null;
        }
        else
        {
            return base.Enter(prevState);
        }
    }

    public override MovementState3D Exit(State _)
    {
        if (_connected)
        {
            _connected = false;
            _animator.AnimationFinished -= BaseEnterMethod;
        }

        return base.Exit(_);
    }

    public override MovementState3D ProcessPhysics(double delta)
    {
        base.ProcessPhysics(delta);
        _agent.Velocity += _agent.GetGravity() * (float)delta;

        if (_agent.IsOnFloor())
        {
            return this.GetInputDirection() == Vector2.Zero ? idleState : walkState;
        }

        return null;
    }
}
