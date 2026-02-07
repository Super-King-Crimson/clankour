using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [Export] public float maxSpeedGrounded = 25f;
    [Export] public float groundedAcceleration = 2f;
    [Export] public float groundedDeceleration = 10f;
    [Export] public float baseSpeed = 10f;

    private AnimationPlayer _animator;
    private CameraMount _cameraMount;
    private StateMachine _stateMachine;
    private PlayerMover _mover;

    [Export] public float jumpVelocity = 4.5f;

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
        _cameraMount = GetNode<CameraMount>("Character/CameraMount");
        _stateMachine = GetNode<StateMachine>("StateMachine");
        _mover = GetNode<PlayerMover>("StateMachine/Mover");

        _stateMachine.Init(this, _animator);
        _mover.Init(_cameraMount);
    }
}
