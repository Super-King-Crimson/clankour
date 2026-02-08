using Godot;
using System;

public abstract partial class GroundedMovementState3D : MovementState3D
{
    [Export] public MovementState3D idleState;
    [Export] public MovementState3D airborneState;
    [Export] public MovementState3D jumpState;

    [Export] public virtual float CurrentSpeed { get; set; } = 0.0f;
    [Export] public virtual float Acceleration { get; set; } = 20.0f;
    [Export] public virtual float RotationSpeed { get; set; } = 0.1f;
    [Export] public virtual float MaxSpeed { get; set; } = 100.0f;
    [Export] public virtual float MaxCoyoteTimeMs { get; set; } = 150.0f;
    [Export] protected virtual Node3D Character { get; set; }

    protected float _coyoteTimerMs = 0.0f;
    protected float _runSpeed = 5.0f;

    protected MovementState3D GetAerialState(double delta)
    {
        if (!_agent.IsOnFloor())
        {
            _coyoteTimerMs += (float)delta;

            if (_coyoteTimerMs > MaxCoyoteTimeMs)
            {
                _coyoteTimerMs = 0.0f;
                return airborneState;
            }

            return null;
        }
        _coyoteTimerMs = 0;

        if (this.WantsJump())
            return jumpState;

        return null;
    }
}
