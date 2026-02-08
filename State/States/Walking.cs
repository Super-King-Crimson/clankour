using Godot;
using System;

public partial class Walking : GroundedMovementState3D
{
    [Export] public Running runState;

    public override MovementState3D Enter(State _)
    {
        CurrentSpeed = _agent.Velocity.Length();

        if (CurrentSpeed > _runSpeed)
            return runState;

        return base.Enter(_);
    }

    public override MovementState3D ProcessPhysics(double delta)
    {
        base.ProcessPhysics(delta);

        if (this.GetAerialState(delta) is MovementState3D aerialState)
            return aerialState;

        Vector2 inputDir = this.GetInputDirection();
        if (inputDir == Vector2.Zero)
        {
            this.CurrentSpeed = 0;
            _agent.Velocity = Vector3.Zero;
            return idleState;
        }

        Vector3 direction = _agent.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y);
        var prevVel = _agent.Velocity;
        var deltaV = Acceleration * (float)delta;
        CurrentSpeed = Math.Min(MaxSpeed, CurrentSpeed + deltaV);
        var goalVel = direction * CurrentSpeed;
        _agent.Velocity = prevVel.Slerp(goalVel, RotationSpeed);

        if (Character is not null)
            Character.LookAt(_agent.Position + _agent.Velocity);

        if (_agent.Velocity.Length() > _runSpeed)
            return runState;

        return null;
    }
}
