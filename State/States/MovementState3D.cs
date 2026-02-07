using Godot;
using System;

public abstract partial class MovementState3D : State
{
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

    public override void Enter()
    {
        _animator.Play(this.animationName);
    }

    public override void Exit()
    {
        if (_animator.CurrentAnimation == this.animationName)
            _animator.Stop();
    }

    public virtual MovementState3D ProcessFrame(double delta) => null;
    public virtual MovementState3D ProcessInput(InputEvent e) => null;

    public virtual MovementState3D ProcessPhysics(double delta)
    {
        _agent.MoveAndSlide();
        return null;
    }

    public virtual Vector2 GetInputDirection() => _mover.GetInputDirection();
    public virtual bool WantsJump() => _mover.WantsJump();
}
