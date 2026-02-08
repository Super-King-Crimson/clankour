using Godot;
using System;

public partial class Walking : MovementState3D
{
    [Export] public MovementState3D idleState;
    [Export] public MovementState3D airborneState;
    [Export] public MovementState3D jumpState;

    [Export] public double currentSpeed = 0.0;
    [Export] public float acceleration = 20.0f;
    [Export] public float friction = 20.0f;

    [Export] public double minimumSlideDotProd = -0.2;
    [Export] public float rotationSpeed = 0.1f;

    [Export] public Node3D character;

    public override MovementState3D ProcessPhysics(double delta)
    {
        base.ProcessPhysics(delta);

        if (!_agent.IsOnFloor())
            return airborneState;

        if (this.WantsJump())
            return jumpState;

        Vector2 inputDir = this.GetInputDirection();
        Vector3 direction = _agent.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y);

        var prevVel = _agent.Velocity;

        if (inputDir == Vector2.Zero)
        {
            if (!prevVel.IsZeroApprox())
            {
                var deltaV2 = friction * (float)delta;
                _agent.Velocity = prevVel.Normalized() * Math.Max(0, prevVel.Length() - deltaV2);

                GD.Print($"before it was {prevVel}, now {_agent.Velocity}");

                if (_agent.Velocity != Vector3.Zero)
                    return null;
            }

            return idleState;
        }

        // TODO: Change to slide state
        if (!prevVel.IsZeroApprox() && prevVel.Normalized().Dot(direction) < minimumSlideDotProd) return idleState;

        var deltaV = acceleration * (float)delta;
        var goalVel = direction * (prevVel.Length() + deltaV);

        _agent.Velocity = prevVel.Slerp(goalVel, rotationSpeed);

        if (character is not null)
            character.LookAt(_agent.Position + _agent.Velocity);

        return null;
    }
}
