using Godot;
using System;

public partial class Sliding : MovementState3D
{
    public override float MinSpeed { get; set; } = 0;
    public override float Friction { get; set; } = 50;

    public float CartwheelSpeed { get; set; } = 10;

    [Export] public float minimumSlideDotProd = -0.2f;

    [Export] private MovementState3D _airborneState;
    [Export] private MovementState3D _idleState;
    [Export] private MovementState3D _jumpState;
    [Export] private MovementState3D _runState;
    [Export] private MovementState3D _walkState;

    protected override MovementState3D GetNextState()
    {
        // check grounded state first in this case
        // so we get input first
        if (this.GetNextGroundedState() is MovementState3D gs) return gs;
        if (this.GetNextAerialState() is MovementState3D @as) return @as;

        return null;
    }

    protected override MovementState3D GetNextGroundedState()
    {
        Vector3 direction = this.GetInputDirection();

        Vector3 velocity = _agent.Velocity;

        if (direction == Vector3.Zero)
        {
            return _idleState;
        }
        GD.Print(Speed);

        if (Speed <= MinSpeed)
        {
            return _walkState;
        }

        if (velocity.Normalized().Dot(direction) >= this.minimumSlideDotProd)
        {
            return _runState;
        }

        return null;
    }

    protected override MovementState3D GetNextAerialState()
    {
        if (!_agent.IsOnFloor())
        {
            return _airborneState;
        }

        if (this.WantsJump())
        {
            Vector3 direction = this.GetInputDirection();
            if (direction != Vector3.Zero)
            {
                _agent.Velocity = direction * CartwheelSpeed;
            }

            return _jumpState;
        }

        return null;
    }

    public override State Enter(State prevState)
    {
        Speed = _agent.Velocity.Length();

        if (this.GetNextState() is State s) return s;

        return base.Enter(prevState);
    }

    public override State ProcessPhysics(double delta)
    {
        if (this.GetNextState() is State s) return s;

        Vector3 prevVel = _agent.Velocity;

        var deltaV = Friction * (float)delta;
        Speed = Math.Max(0, Speed - deltaV);
        _agent.Velocity = prevVel.Normalized() * Speed;

        return base.ProcessPhysics(delta);
    }

    public Sliding() : base("sliding") { }
}
