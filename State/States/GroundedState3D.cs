using Godot;
using System;

public abstract partial class GroundedState3D : MovementState3D
{
    [Export] public MovementState3D airborneState;
    [Export] public MovementState3D jumpState;

    [Export] public virtual float Acceleration { get; set; } = 10.0f;
    [Export] public virtual float RotationSpeed { get; set; } = 0.1f;
    [Export] public virtual float MaxSpeed { get; set; } = 20.0f;
    [Export] protected virtual Node3D Character { get; set; }

    protected float _runSpeed = 10.0f;

    protected MovementState3D GetAerialState(double delta)
    {
        if (!_agent.IsOnFloor())
        {
            return airborneState;
        }

        if (this.WantsJump())
            return jumpState;

        return null;
    }
}
