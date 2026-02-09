using Godot;
using System;

public partial class Player : CharacterBody3D
{
    private AnimationPlayer _animator;
    private StateMachine _stateMachine;
    private PlayerMover _mover;

    public override void _PhysicsProcess(double delta)
    {
        _stateMachine.ProcessPhysics(delta);
    }

    public override void _Process(double delta)
    {
        _stateMachine.ProcessFrame(delta);
    }

    public override void _UnhandledInput(InputEvent e)
    {
        _stateMachine.ProcessInput(e);
    }

    public override void _Ready()
    {
        _animator = GetNode<AnimationPlayer>("Character/Model/Animator");
        _stateMachine = GetNode<StateMachine>("StateMachine");
        _mover = GetNode<PlayerMover>("StateMachine/Mover");

        _stateMachine.Init(this, _animator);
    }
}
