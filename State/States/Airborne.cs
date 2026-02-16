using Godot;
using System;

public partial class Airborne : MovementState3D
{
    [Export] public override float Acceleration { get; set; } = 3;
    [Export] public override float MaxSpeed { get; set; } = 20;
    [Export] public override float RotationSpeed { get; set; } = 500;

    [Export] private MovementState3D _walkState;
    [Export] private MovementState3D _jumpState;

    [Export] public float MinNoDecelerateDot { get; set; } = 0;

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
        // assume player doesn't have coyote time 
        // until proven otherwise
        _coyoteTimer = this.coyoteTime;

        if (this.GetNextState() is State s) return s;

        if (prevState is Jumping)
        {
            _connected = true;
            _animator.AnimationFinished += this.StartAnimationOnFinished;

            return null;
        }
        else
        {
            _coyoteTimer = 0;

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
            float deltaV = Acceleration * fdelta;

            var speed = velocityXZ.Length();

            if (speed == 0)
            {
                velocityXZ = directionXZ;
            }

            Vector2 xzNorm = velocityXZ.Normalized();
            Vector3 newVel;

            if (velocityXZ.Dot(directionXZ) < MinNoDecelerateDot)
            {
                speed -= deltaV;

                newVel = new Vector3(xzNorm.X * speed, 0, xzNorm.Y * speed);
            }
            else
            {
                speed = Math.Min(MaxSpeed, speed + deltaV);

                newVel = speed * this.RotateFromRotationSpeed(
                    new Vector3(xzNorm.X, 0, xzNorm.Y),
                    directionNorm,
                    fdelta
                );
            }

            newVel.Y = _agent.Velocity.Y;
            _agent.Velocity = newVel;

            if (_character is not null)
            {
                GD.Print(directionNorm);
                _character.LookAt(_agent.Position + directionNorm);
            }
        }

        _agent.Velocity += _agent.GetGravity() * fdelta;

        return base.ProcessPhysics(delta);
    }

    public Airborne() : base("airborne") { }
}
