using Godot;
using System;

public abstract partial class MovementState3D : State
{
    public virtual float Acceleration { get; set; } = 0;
    public virtual float Friction { get; set; } = 0;
    public virtual float MaxSpeed { get; set; } = float.MaxValue;
    public virtual float MinSpeed { get; set; } = 0;
    public virtual float RotationSpeed { get; set; } = 0;
    public virtual float Speed { get; set; } = 0;

    public virtual float AnimSpeedBase { get; set; } = 1;
    public virtual float AnimSpeedScale { get; set; } = 0;

    protected CharacterBody3D _agent;
    protected Node3D _character;
    protected AnimationPlayer _animator;
    protected IMover _mover;

    [Export] public string animationName;

    public void Init(CharacterBody3D agent, AnimationPlayer animator, IMover mover)
    {
        _agent = agent;
        _animator = animator;
        _mover = mover;
    }

    public void SetCharacter(Node3D character)
    {
        _character = character;
    }

    protected virtual MovementState3D GetNextAerialState() => null;
    protected virtual MovementState3D GetNextGroundedState() => null;

    protected virtual MovementState3D GetNextState()
    {
        MovementState3D nextState;

        nextState = this.GetNextAerialState();
        if (nextState is not null)
        {
            return nextState;
        }

        nextState = this.GetNextGroundedState();
        if (nextState is not null)
        {
            return nextState;
        }

        return null;
    }

    public virtual float GetAnimationSpeed() => AnimSpeedBase;

    public virtual void StartAnimation()
    {
        _animator.SpeedScale = this.GetAnimationSpeed();
        _animator.Play(this.animationName);
    }

    public virtual Vector3 GetInputDirection()
    {
        Vector2 input = this._mover.GetInputDirection();
        Vector3 input3D = new(input.X, 0, input.Y);

        return _agent.Transform.Basis * input3D;
    }

    public virtual bool WantsJump() => _mover.WantsJump();

    public override State Enter(State _)
    {
        this.StartAnimation();

        return null;
    }

    public override State Exit(State _) => null;

    public override State ProcessFrame(double delta) => null;
    public override State ProcessInput(InputEvent e) => null;

    public override State ProcessPhysics(double delta)
    {
        _agent.MoveAndSlide();

        return null;
    }

    public MovementState3D(string id) : base(id) { }
}
