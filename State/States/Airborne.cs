using Godot;
using System;

public partial class Airborne : MovementState3D
{
    [Export] public override float Acceleration { get; set; } = 3.0f;
    [Export] public override float Friction { get; set; } = 20.0f;
    [Export] public override float MaxSpeed { get; set; } = 5.0f;
    [Export] public override float RotationSpeed { get; set; } = 1.0f;

    [Export] private MovementState3D _walkState;
    [Export] private MovementState3D _jumpState;

    private float _coyoteTimer = 0;
    [Export] public float coyoteTime = 0.25f;

    private bool _connected = false;
    private void StartAnimationOnFinished(StringName _)
    {
        this.StartAnimation();
    }

    protected override MovementState3D GetNextGroundedState()
    {
        if (_agent.IsOnFloor())
            return _walkState;

        return null;
    }

    protected override MovementState3D GetNextAerialState()
    {
        if (_coyoteTimer < this.coyoteTime)
        {
            if (this.WantsJump())
                return _jumpState;
        }

        return null;
    }

    public override State Enter(State prevState)
    {
        _coyoteTimer = 0;

        if (prevState is Jumping)
        {
            _coyoteTimer = this.coyoteTime;

            if (this.GetNextState() is State s) return s;

            _connected = true;
            _animator.AnimationFinished += this.StartAnimationOnFinished;

            return null;
        }
        else
        {
            if (this.GetNextState() is State s) return s;

            return base.Enter(prevState);
        }
    }

    public override State Exit(State _)
    {
        if (_connected)
        {
            _connected = false;
            _animator.AnimationFinished -= this.StartAnimationOnFinished;
        }

        return base.Exit(_);
    }

    public override State ProcessPhysics(double delta)
    {
        if (this.GetNextState() is MovementState3D s) return s;

        float fdelta = (float)delta;
        _coyoteTimer += fdelta;

        Vector3 directionNorm = this.GetInputDirection().Normalized();

        if (directionNorm != Vector3.Zero)
        {
            var velocityXZ = new Vector2(_agent.Velocity.X, _agent.Velocity.Z);
            var directionXZ = new Vector2(directionNorm.X, directionNorm.Z);
            Vector2 targetXZ = velocityXZ.Normalized().Slerp(directionXZ, RotationSpeed * fdelta);

            float deltaV = Acceleration * fdelta;
            float accelerationAtt = velocityXZ.Length() + deltaV;
            float newSpeed = Math.Min(MaxSpeed, accelerationAtt);

            var newVel = new Vector3(targetXZ.X * newSpeed, _agent.Velocity.Y, targetXZ.Y * newSpeed);

            _agent.Velocity = newVel;

            if (_character is not null)
            {
                newVel.Y = 0;
                _character.LookAt(_agent.Position + newVel);
            }
        }

        _agent.Velocity += _agent.GetGravity() * fdelta;

        return base.ProcessPhysics(delta);
    }

    public Airborne() : base("airborne") { }
}
