using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [Export] public float camVerticalConstraintDeg = 60f;
    [Export] public float maxSpeedGrounded = 25f;
    [Export] public float groundedAcceleration = 2f;
    [Export] public float groundedDeceleration = 10f;
    [Export] public float baseSpeed = 10f;

    private Node3D _cameraMount;
    private Node3D _character;
    private AnimationPlayer _animationPlayer;

    private StateMachine _stateMachine;

    [Export] public float jumpVelocity = 4.5f;
    [Export] public float camSensH = 0.5f;
    [Export] public float camSensV = 0.5f;
    [Export] public bool camInvertH = true;
    [Export] public bool camInvertV = true;

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;

        if (!IsOnFloor())
            velocity += GetGravity() * (float)delta;

        if (Input.IsActionJustPressed("jump") && IsOnFloor())
            velocity.Y = jumpVelocity;

        Vector2 inputDir = Input.GetVector("left", "right", "fore", "back");

        Vector3 direction = Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y);

        float speed = baseSpeed;

        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * speed;
            velocity.Z = direction.Z * speed;

            _animationPlayer.Play("walking");
            _character.LookAt(this.Position + direction);
        }
        else
        {
            velocity.X = 0;
            velocity.Z = 0;

            _animationPlayer.Play("idle");
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseMotion motionEvent)
        {
            int invH = this.camInvertH ? -1 : 1;
            int invV = this.camInvertV ? -1 : 1;

            this.RotateY(invH * camSensH * Mathf.DegToRad(motionEvent.Relative.X));
            _cameraMount.RotateX(invV * camSensV * Mathf.DegToRad(motionEvent.Relative.Y));

            _cameraMount.RotationDegrees = new Vector3(
                Mathf.Clamp(_cameraMount.RotationDegrees.X, -1 * camVerticalConstraintDeg, camVerticalConstraintDeg),
                _cameraMount.RotationDegrees.Y,
                _cameraMount.RotationDegrees.Z
            );
        }
    }

    public override void _Ready()
    {
        _cameraMount = GetNode<Node3D>("CameraMount");
        _animationPlayer = GetNode<AnimationPlayer>("Character/Model/AnimationPlayer");
        _character = GetNode<Node3D>("Character");
        _stateMachine = GetNode<StateMachine>("StateMachine");

        _stateMachine.Init(this, _animationPlayer);
    }
}
