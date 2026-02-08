using Godot;
using System;

public partial class Running : GroundedMovementState3D
{
    [Export] public Walking walkState;

    [Export] public override float Acceleration { get; set; } = 30.0f;
    [Export] public float minimumSlideDotProd = -0.2f;
    [Export] public float friction = 10.0f;

    public override MovementState3D Enter(State _)
    {
        CurrentSpeed = _agent.Velocity.Length();

        if (CurrentSpeed < _runSpeed)
            return walkState;

        return base.Enter(_);
    }

    public override MovementState3D ProcessPhysics(double delta)
    {
        base.ProcessPhysics(delta);

        if (this.GetAerialState(delta) is MovementState3D aerialState)
            return aerialState;

        Vector2 inputDir = this.GetInputDirection();
        Vector3 prevVel = _agent.Velocity;

        if (inputDir == Vector2.Zero)
        {
            if (CurrentSpeed != 0)
            {
                var deltaV2 = friction * (float)delta;
                CurrentSpeed = Math.Max(0, CurrentSpeed - deltaV2);
                _agent.Velocity = prevVel.Normalized() * CurrentSpeed;

                if (CurrentSpeed != 0)
                    return null;
            }

            return idleState;
        }

        Vector3 direction = _agent.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y);

        // TODO: Change to slide state
        if (CurrentSpeed != 0 && prevVel.Normalized().Dot(direction) < minimumSlideDotProd) return idleState;

        var deltaV = Acceleration * (float)delta;
        CurrentSpeed = Math.Min(MaxSpeed, CurrentSpeed + deltaV);
        var goalVel = direction * CurrentSpeed;
        _agent.Velocity = prevVel.Slerp(goalVel, RotationSpeed);

        if (Character is not null)
            Character.LookAt(_agent.Position + _agent.Velocity);

        return null;
    }
}


