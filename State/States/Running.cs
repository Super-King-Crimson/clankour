using Godot;
using System;

public partial class Running : GroundedState3D
{
    [Export] public GroundedState3D walkState;
    [Export] public MovementState3D idleState;

    [Export] public override float Acceleration { get; set; } = 10.0f;
    [Export] public float minimumSlideDotProd = -0.2f;
    [Export] public float friction = 50.0f;

    public override MovementState3D Enter(State _)
    {
        var speed = _agent.Velocity.Length();

        if (speed < _runSpeed)
            return walkState;

        return base.Enter(_);
    }

    public override MovementState3D ProcessPhysics(double delta)
    {
        base.ProcessPhysics(delta);

        if (this.GetAerialState(delta) is MovementState3D aerialState)
        {
            GD.Print($"I am moving at {_agent.Velocity}");
            GD.Print($"I am located at {_agent.Position}");
            return aerialState;

        }
        Vector2 inputDir = this.GetInputDirection();
        Vector3 prevVel = _agent.Velocity;
        var speed = prevVel.Length();
        float newSpeed;
        float deltaV;

        if (inputDir == Vector2.Zero)
        {
            if (speed != 0)
            {
                deltaV = friction * (float)delta;
                newSpeed = Math.Max(0, speed - deltaV);
                _agent.Velocity = prevVel.Normalized() * newSpeed;

                if (speed != 0)
                    return null;
            }

            return idleState;
        }

        Vector3 direction = _agent.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y);

        // TODO: Change to slide state
        if (speed != 0 && prevVel.Normalized().Dot(direction) < minimumSlideDotProd)
            return idleState;

        deltaV = Acceleration * (float)delta;
        newSpeed = Math.Min(MaxSpeed, speed + deltaV);

        newSpeed = Math.Min(speed + deltaV, MaxSpeed);
        var newVel = prevVel.Normalized() * newSpeed;
        var goalVel = direction * newSpeed;
        _agent.Velocity = newVel.Slerp(goalVel, RotationSpeed);

        if (Character is not null)
            Character.LookAt(_agent.Position + _agent.Velocity);

        return null;
    }
}


