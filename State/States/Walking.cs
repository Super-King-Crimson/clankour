using Godot;
using System;

public partial class Walking : MovementState3D
{
    [Export] public MovementState3D idleState;
    [Export] public MovementState3D airborneState;
    [Export] public MovementState3D jumpState;

    [Export] public float baseSpeed = 5f;
    [Export] public float currentSpeed = 0f;

    [Export] public Node3D character;

    public override MovementState3D ProcessPhysics(double delta)
    {
        if (!_agent.IsOnFloor())
            return airborneState;

        if (this.WantsJump())
            return jumpState;

        Vector2 inputDir = this.GetInputDirection();
        if (inputDir == Vector2.Zero)
            return idleState;

        Vector3 direction = _agent.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y);

        // optionally model face towards direction player is moving
        if (character is not null)
            character.LookAt(_agent.Position + direction);

        _agent.Velocity = direction * baseSpeed;

        return base.ProcessPhysics(delta);
    }
}
