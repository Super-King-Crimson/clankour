using Godot;
using System;

public partial class Running : MovementState3D
{
    [Export] public override float Acceleration { get; set; } = 7;
    [Export] public override float RotationSpeed { get; set; } = 20;

    [Export] public override float MinSpeed { get; set; } = 10;
    [Export] public override float MaxSpeed { get; set; } = 30;

    [Export] public override float AnimSpeedBase { get; set; } = 0.5f;
    [Export] public override float AnimSpeedScale { get; set; } = 1;

    [Export] private MovementState3D _airborneState;
    [Export] private MovementState3D _idleState;
    [Export] private MovementState3D _jumpState;
    [Export] private Sliding _slidingState;
    [Export] private MovementState3D _walkState;

    protected override MovementState3D GetNextGroundedState()
    {
        Vector3 direction = this.GetInputDirection();

        if (direction == Vector3.Zero)
            return _idleState;

        if (_agent.Velocity.Normalized().Dot(direction) <= _slidingState.minimumSlideDotProd)
            return _slidingState;

        if (Speed < MinSpeed)
            return _walkState;

        return null;
    }

    protected override MovementState3D GetNextAerialState()
    {
        if (this.WantsJump())
            return _jumpState;

        if (!_agent.IsOnFloor())
            return _airborneState;

        return null;
    }

    public override State Enter(State prevState)
    {
        Speed = _agent.Velocity.Length();

        if (this.GetNextState() is State s) return s;

        return base.Enter(prevState);
    }

    public override float GetAnimationSpeed() => AnimSpeedBase + (AnimSpeedScale * (Speed / MaxSpeed));

    public override State ProcessPhysics(double delta)
    {
        if (this.GetNextState() is State s) return s;

        Vector3 direction = this.GetInputDirection();

        Vector3 prevVel = _agent.Velocity;

        float fdelta = (float)delta;
        float deltaV = Acceleration * fdelta;

        float agentSpeed = _agent.Velocity.Length();
        Speed = Math.Min(MaxSpeed, agentSpeed + deltaV);

        var newVel = prevVel.Normalized() * Speed;
        var goalVel = direction * Speed;
        _agent.Velocity = newVel.Slerp(goalVel, RotationSpeed * fdelta);

        _animator.SpeedScale = this.GetAnimationSpeed();

        if (_character is not null)
            _character.LookAt(_agent.Position + _agent.Velocity);

        return base.ProcessPhysics(delta);
    }

    public Running() : base("running") { }
}
