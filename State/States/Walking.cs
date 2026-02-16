using Godot;
using System;

public partial class Walking : MovementState3D
{
    [Export] public override float Acceleration { get; set; } = 5;
    [Export] public override float MinSpeed { get; set; } = 1;
    [Export] public override float RotationSpeed { get; set; } = 500;

    [Export] public override float AnimSpeedBase { get; set; } = 0.5f;
    [Export] public override float AnimSpeedScale { get; set; } = 2;

    [Export] public float MaxSpeedNoDot { get; set; } = 5;
    [Export] public float MinAccelerationDot { get; set; } = 0.5f;

    [Export] private MovementState3D _airborneState;
    [Export] private MovementState3D _idleState;
    [Export] private MovementState3D _jumpState;
    [Export] private MovementState3D _runState;

    protected override MovementState3D GetNextGroundedState()
    {
        if (this.GetInputDirection() == Vector3.Zero)
            return _idleState;

        if (Speed > _runState.MinSpeed)
            return _runState;

        return null;
    }

    protected override MovementState3D GetNextAerialState()
    {
        if (!_agent.IsOnFloor())
            return _airborneState;

        if (this.WantsJump())
            return _jumpState;

        return null;
    }

    public override State Enter(State prevState)
    {
        Speed = _agent.Velocity.Length();

        if (this.GetNextState() is State s) return s;

        return base.Enter(prevState);
    }

    public override float GetAnimationSpeed() => AnimSpeedBase + (AnimSpeedScale * ((Speed - MinSpeed) / _runState.MinSpeed));

    public override State ProcessPhysics(double delta)
    {
        if (this.GetNextState() is State s) return s;

        Vector3 directionNorm = this.GetInputDirection().Normalized();
        Vector3 prevVel = _agent.Velocity;

        Speed = _agent.Velocity.Length();

        if (Speed < MinSpeed)
        {
            if (Speed == 0)
            {
                prevVel = directionNorm;
            }

            Speed = MinSpeed;
        }

        Vector3 prevVelNorm = prevVel.Normalized();

        float newSpeed = Speed;
        float fdelta = (float)delta;

        if (prevVelNorm.Dot(directionNorm) >= MinAccelerationDot || Speed < MaxSpeedNoDot)
        {
            newSpeed += fdelta * Acceleration;
        }
        else
        {
            newSpeed = Math.Min(MaxSpeedNoDot, Speed);
        }

        _agent.Velocity = this.RotateFromRotationSpeed(prevVelNorm, directionNorm, fdelta) * newSpeed;

        if (_character is not null)
            _character.LookAt(_agent.Position + _agent.Velocity);

        _animator.SpeedScale = this.GetAnimationSpeed();

        return base.ProcessPhysics(delta);
    }

    public Walking() : base("walking") { }
}

