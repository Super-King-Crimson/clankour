using Godot;
using System;

public partial class Walking : GroundedState3D
{
    public override float RotationSpeed { get; set; } = 10.0f;

    public override float AnimSpeedBase { get; set; } = 0.5f;
    public override float AnimSpeedScale { get; set; } = 0.5f;

    [Export] public GroundedState3D runState;
    [Export] public GroundedState3D idleState;

    protected override MovementState3D GetGroundedState()
    {
        if (_agent.Velocity.Length() > GroundedState3D.RunSpeed)
            return this.runState;

        return null;
    }

    public override MovementState3D ProcessPhysics(double delta)
    {
        base.ProcessPhysics(delta);

        if (this.GetAerialState() is MovementState3D aerialState)
        {
            return aerialState;
        }

        if (this.GetGroundedState() is MovementState3D groundedState)
        {
            return groundedState;
        }

        Vector3 direction = this.GetInputDirection();
        if (direction == Vector3.Zero)
        {
            _agent.Velocity = Vector3.Zero;
            return idleState;
        }

        float fdelta = (float)delta;

        var prevVel = _agent.Velocity;
        var speed = prevVel.Length();
        var deltaV = Acceleration * fdelta;

        var newSpeed = Math.Min(speed + deltaV, MaxSpeed);
        var newVel = prevVel.Normalized() * newSpeed;
        var goalVel = direction * newSpeed;
        _agent.Velocity = newVel.Slerp(goalVel, RotationSpeed * fdelta);

        if (Character is not null)
            Character.LookAt(_agent.Position + _agent.Velocity);

        _animator.SpeedScale = this.GetAnimationSpeed(newSpeed);

        return null;
    }
}
