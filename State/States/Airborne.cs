using Godot;
using System;

public partial class Airborne : MovementState3D
{
    [Export] public MovementState3D walkState;

    private void BaseEnterMethod(StringName _) => base.Enter(null);
    private bool _connected = false;

    public override void Enter(State prevState)
    {
        if (prevState is Jumping)
        {
            _connected = true;
            _animator.AnimationFinished += BaseEnterMethod;
        }
        else
        {
            base.Enter(prevState);
        }
    }

    public override void Exit(State _)
    {
        if (_connected)
        {
            _connected = false;
            _animator.AnimationFinished -= BaseEnterMethod;
        }

        base.Exit(_);
    }

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
