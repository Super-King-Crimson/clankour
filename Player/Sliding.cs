using Godot;
using System;

public partial class Sliding : GroundedState3D
{
    [Export] public GroundedState3D walkState;
    [Export] public GroundedState3D runState;
    [Export] public GroundedState3D idleState;

    public override float Friction { get; set; } = 40.0f;
    public float cartwheelBoost = 3.0f;

    protected override MovementState3D GetGroundedState()
    {
        Vector3 direction = this.GetInputDirection();

        Vector3 velocity = _agent.Velocity;

        if (velocity.Normalized().Dot(direction) > GroundedState3D.MinimumSlideDotProd)
            return velocity.Length() > GroundedState3D.RunSpeed
                ? runState
                : walkState;

        if (velocity.Length() < this.MinSpeed)
            return idleState;

        return null;
    }

    public override MovementState3D ProcessPhysics(double delta)
    {
        base.ProcessPhysics(delta);

        if (this.GetAerialState() is MovementState3D aerialState)
        {
            if (aerialState is Jumping)
            {
                Vector3 direction = this.GetInputDirection();

                Vector3 newVelocity = direction * _agent.Velocity.Length();
                newVelocity.Y += this.cartwheelBoost;

                _agent.Velocity = newVelocity;
            }
            return aerialState;
        }

        if (this.GetGroundedState() is MovementState3D groundedState)
        {
            return groundedState;
        }

        Vector3 prevVel = _agent.Velocity;
        float speed = prevVel.Length();

        var deltaV = Friction * (float)delta;
        float newSpeed = Math.Max(0, speed - deltaV);
        _agent.Velocity = prevVel.Normalized() * newSpeed;

        return null;
    }
}
