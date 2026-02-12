using Godot;
using System;

public partial class Idle : MovementState3D
{
    [Export] public override float Friction { get; set; } = 30.0f;

    [Export] private MovementState3D _airborneState;
    [Export] private MovementState3D _jumpState;
    [Export] private MovementState3D _walkState;

    protected override MovementState3D GetNextGroundedState()
    {
        if (this.GetInputDirection() != Vector3.Zero)
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

    public override State ProcessPhysics(double delta)
    {
        if (this.GetNextState() is State s) return s;

        Vector3 prevVel = _agent.Velocity;
        var speed = prevVel.Length();

        float deltaV = Friction * (float)delta;
        float newSpeed = Math.Max(0, speed - deltaV);

        _agent.Velocity = prevVel.Normalized() * newSpeed;

        return base.ProcessPhysics(delta);
    }

    public Idle() : base("idle") { }
}
