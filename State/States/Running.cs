using Godot;
using System;

public partial class Running : GroundedState3D
{
    public override float AnimSpeedBase { get; set; } = 0.5f;
    public override float AnimSpeedScale { get; set; } = 1.0f;

    [Export] public GroundedState3D walkState;
    [Export] public GroundedState3D idleState;
    [Export] public GroundedState3D slidingState;

    [Export] public override float Acceleration { get; set; } = 7.0f;

    protected override MovementState3D GetGroundedState()
    {
        var direction = this.GetInputDirection();

        Vector3 velocity = _agent.Velocity;

        if (velocity.Normalized().Dot(direction) < GroundedState3D.MinimumSlideDotProd)
            return slidingState;

        if (velocity.Length() < this.MinSpeed)
            return walkState;

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
        Vector3 prevVel = _agent.Velocity;
        var speed = prevVel.Length();
        float newSpeed;
        float deltaV;

        if (direction == Vector3.Zero)
        {
            deltaV = Friction * (float)delta;
            newSpeed = Math.Max(0, speed - deltaV);
            _agent.Velocity = prevVel.Normalized() * newSpeed;

            if (speed != 0)
                return null;

            return idleState;
        }

        deltaV = Acceleration * (float)delta;
        newSpeed = Math.Min(MaxSpeed, speed + deltaV);

        newSpeed = Math.Min(speed + deltaV, MaxSpeed);
        var newVel = prevVel.Normalized() * newSpeed;
        var goalVel = direction * newSpeed;
        _agent.Velocity = newVel.Slerp(goalVel, RotationSpeed);

        _animator.SpeedScale = this.GetAnimationSpeed(newSpeed);

        if (Character is not null)
            Character.LookAt(_agent.Position + _agent.Velocity);

        return null;
    }
}


