using Godot;
using System;

public partial class Airborne : MovementState3D
{
    [Export] public GroundedState3D walkState;
    [Export] public MovementState3D idleState;
    [Export] public MovementState3D jumpState;
    [Export] public float coyoteTime = 0.25f;

    private void BaseEnterMethod(StringName _) => base.Enter(null);
    private bool _connected = false;
    private float _coyoteTimer = 0.0f;

    public override MovementState3D Enter(State prevState)
    {
        _coyoteTimer = 0.0f;

        if (prevState is Jumping)
        {
            _coyoteTimer = this.coyoteTime;

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

        float fdelta = (float)delta;

        _coyoteTimer += fdelta;
        _agent.Velocity += _agent.GetGravity() * fdelta;

        if (_agent.IsOnFloor())
            return this.GetInputDirection() == Vector2.Zero ? idleState : walkState;

        if (_coyoteTimer < this.coyoteTime)
        {
            if (this.WantsJump())
                return jumpState;
        }

        return null;
    }
}
