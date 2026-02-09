using Godot;
using System;

public partial class Airborne : MovementState3D
{
    [Export] public float Acceleration { get; set; } = 3.0f;
    [Export] public float Friction { get; set; } = 20.0f;
    [Export] public float MaxSpeed { get; set; } = 5.0f;
    [Export] public float RotationSpeed { get; set; } = 1.0f;

    [Export] public GroundedState3D walkState;
    [Export] public MovementState3D idleState;
    [Export] public MovementState3D jumpState;
    [Export] public float coyoteTime = 0.25f;

    private void BaseEnterMethod(StringName _) => base.Enter(null);
    private bool _connected = false;
    private float _coyoteTimer = 0.0f;

    public override State Enter(State prevState)
    {
        _coyoteTimer = 0.0f;

        if (prevState is Jumping)
        {
            _coyoteTimer = this.coyoteTime;

            _connected = true;
            _animator.AnimationFinished += BaseEnterMethod;

            return null;
        }
        else
        {
            return base.Enter(prevState);
        }
    }

    public override State Exit(State _)
    {
        if (_connected)
        {
            _connected = false;
            _animator.AnimationFinished -= BaseEnterMethod;
        }

        return base.Exit(_);
    }

    public override MovementState3D ProcessPhysics(double delta)
    {
        base.ProcessPhysics(delta);

        float fdelta = (float)delta;
        _coyoteTimer += fdelta;

        Vector3 directionNorm = this.GetInputDirection().Normalized();
        if (directionNorm != Vector3.Zero)
        {
            Vector2 velocityXZ = new(_agent.Velocity.X, _agent.Velocity.Z);
            Vector2 directionXZ = new Vector2(directionNorm.X, directionNorm.Z);
            Vector2 targetXZ = velocityXZ.Normalized().Slerp(directionXZ, RotationSpeed * fdelta);

            float deltaV = Acceleration * fdelta;

            // only add the vector if it wouldn't put us over the max
            float accelerationAtt = velocityXZ.Length() + deltaV;
            float newSpeed = Math.Max(velocityXZ.Length(), Math.Min(accelerationAtt, MaxSpeed));

            Vector3 newVel = new(targetXZ.X * newSpeed, _agent.Velocity.Y, targetXZ.Y * newSpeed);

            _agent.Velocity = newVel;

            if (Character is not null && directionXZ != Vector2.Zero)
            {
                newVel.Y = 0;
                Character.LookAt(_agent.Position + newVel);
            }
        }
        else
        {
            float newSpeed = Math.Max(0, _agent.Velocity.Length() - (fdelta * Friction));
            Vector3 scaledVel = _agent.Velocity.Normalized() * newSpeed;

            _agent.Velocity = new(scaledVel.X, _agent.Velocity.Y, scaledVel.Z);
        }

        _agent.Velocity += _agent.GetGravity() * fdelta;

        if (_agent.IsOnFloor())
            return walkState;

        if (_coyoteTimer < this.coyoteTime)
        {
            if (this.WantsJump())
                return jumpState;
        }

        return null;
    }
}
