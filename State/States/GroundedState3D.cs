using Godot;
using System;

public abstract partial class GroundedState3D : MovementState3D
{
    public static float RunSpeed { get; set; } = 5.0f;
    public static float MinimumSlideDotProd { get; set; } = -0.2f;

    [Export] public MovementState3D airborneState;
    [Export] public MovementState3D jumpState;

    [Export] public virtual float Acceleration { get; set; } = 1.0f;
    [Export] public virtual float RotationSpeed { get; set; } = 5.0f;
    [Export] public virtual float MaxSpeed { get; set; } = 20.0f;
    [Export] public virtual float MinSpeed { get; set; } = 1.0f;
    [Export] public virtual float Friction { get; set; } = 20.0f;

    [Export] public virtual float AnimSpeedBase { get; set; } = 1.0f;
    [Export] public virtual float AnimSpeedScale { get; set; } = 0.0f;

    protected float GetAnimationSpeed(float currSpeed) => AnimSpeedBase + (AnimSpeedScale * (currSpeed / MaxSpeed));

    protected virtual MovementState3D GetAerialState()
    {
        if (!_agent.IsOnFloor())
        {
            return airborneState;
        }

        if (this.WantsJump())
            return jumpState;

        return null;
    }

    protected virtual MovementState3D GetGroundedState() => null;


    public override State Enter(State _)
    {
        if (this.GetGroundedState() is GroundedState3D newState)
            return newState;

        return base.Enter(_);
    }

    public override State Exit(State _)
    {
        _animator.SpeedScale = 1;

        return base.Exit(_);
    }
}
