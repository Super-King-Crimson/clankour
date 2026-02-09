using Godot;
using System;

public abstract partial class MovementState3D : State
{
    [Export] protected virtual Node3D Character { get; set; }

    protected CharacterBody3D _agent;
    protected AnimationPlayer _animator;
    protected IMover _mover;
    [Export] public string animationName;

    public void Init(CharacterBody3D agent, AnimationPlayer animator, IMover mover)
    {
        _agent = agent;
        _animator = animator;
        _mover = mover;
    }

    public override State Enter(State _)
    {
        _animator.Play(this.animationName);

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

    public virtual Vector3 GetInputDirection() => _mover.GetInputDirection();
    public virtual bool WantsJump() => _mover.WantsJump();
}
