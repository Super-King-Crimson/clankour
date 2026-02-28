using Godot;
using System;
using Id = SMNodeId;

public partial class Jumping : SMNode
{
    [Export] protected int MidairJumps { get; set; } = 0;
    [Export] protected float JumpVelocity { get; set; } = 4.5f;
    [Export] protected float CartwheelJumpVelocity { get; set; } = 10.0f;

    [Export] protected string CartwheelAnimationName { get; set; } = "ENTER ANIMATION NAME";
    [Export] protected string DefaultAnimationName { get; set; } = "BRO ENTER THE NAME";

    private bool _doingCartwheel = false;
    private bool _jumped = false;


    public Jumping() : base(Id.Jumping) { }

    public override void Enter(SMNode prevState)
    {
        fireEnter(prevState.id);
        _jumped = false;

        if (prevState.id == Id.Sliding)
        {
            _doingCartwheel = true;
            AnimationName = CartwheelAnimationName;
        }
        else
        {
            _doingCartwheel = false;
            AnimationName = DefaultAnimationName;
        }

        playAnimation();
    }

    public override void Exit(SMNode nextState)
    {
        fireExit(nextState.id);
    }

    public override Id? GetTransition()
    {
        if (_jumped)
        {
            return Id.Airborne;
        }
        else
        {
            return null;
        }
    }

    public override void ProcessPhysics(double delta)
    {
        var agent = getAgent();

        if (!_jumped)
        {
            var newVel = getVelocityH();

            float yVelocity = _doingCartwheel ? this.CartwheelJumpVelocity : this.JumpVelocity;

            newVel.Y = yVelocity;
            agent.Velocity = newVel;
        }

        agent.MoveAndSlide();
        _jumped = true;
    }
}
