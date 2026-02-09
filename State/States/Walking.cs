using Godot;
using System;

public partial class Walking : GroundedState3D
{
    [Export] public GroundedState3D runState;
    [Export] public GroundedState3D idleState;

    [Export] public float animSpeedBase = 0.5f;
    [Export] public float animSpeedScale = 1.0f;

    public override MovementState3D Enter(State _)
    {
        var speed = _agent.Velocity.Length();

        if (speed > _runSpeed)
            return runState;

        return base.Enter(_);
    }

    public override MovementState3D Exit(State _)
    {
        _animator.SpeedScale = 1;

        return base.Exit(_);
    }

    public override MovementState3D ProcessPhysics(double delta)
    {
        base.ProcessPhysics(delta);

        if (this.GetAerialState(delta) is MovementState3D aerialState)
            return aerialState;

        Vector2 inputDir = this.GetInputDirection();
        if (inputDir == Vector2.Zero)
        {
            _agent.Velocity = Vector3.Zero;
            return idleState;
        }

        Vector3 direction = _agent.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y);
        var prevVel = _agent.Velocity;
        var speed = prevVel.Length();
        var deltaV = Acceleration * (float)delta;

        var newSpeed = Math.Min(speed + deltaV, MaxSpeed);
        var newVel = prevVel.Normalized() * newSpeed;
        var goalVel = direction * newSpeed;
        _agent.Velocity = newVel.Slerp(goalVel, RotationSpeed);

        _animator.SpeedScale = this.animSpeedBase + (this.animSpeedScale * newSpeed / _runSpeed);

        if (Character is not null)
            Character.LookAt(_agent.Position + _agent.Velocity);

        if (_agent.Velocity.Length() > _runSpeed)
            return runState;

        return null;
    }
}
