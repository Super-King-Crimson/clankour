using Godot;
using System;
using Id = StateMachineNodeId;

public partial class Jumping : StateMachineNode
{
    [Export] public int midairJumps = 0;
    [Export] public float jumpVelocity = 4.5f;

    [Export] protected string _cartwheelAnimationName = null!;
    [Export] protected string _defaultAnimationName = null!;

    private bool _doingCartwheel = false;
    private bool _jumped = false;

    [Export] public float cartwheelJumpVelocity = 10.0f;

    public Jumping() : base(Id.Jumping) { }

    public override void Enter(StateMachineNode prevState)
    {
        _jumped = false;

        if (prevState.id == Id.Sliding)
        {
            _doingCartwheel = true;
            AnimationName = _cartwheelAnimationName;
        }
        else
        {
            _doingCartwheel = false;
            AnimationName = _defaultAnimationName;
        }

        playAnimation();
    }

    public override void Exit(StateMachineNode nextState) { }

    public override bool ExitIfInvalid()
    {
        if (_jumped)
        {
            fireExit(Id.Airborne);
            return true;
        }

        return false;
    }

    public override void ProcessPhysics(double delta)
    {
        var agent = getAgent();
        var newVel = getVelocityH();

        float yVelocity = _doingCartwheel ? this.cartwheelJumpVelocity : this.jumpVelocity;

        newVel.Y = yVelocity;
        agent.Velocity = newVel;
        _jumped = true;

        agent.MoveAndSlide();
    }
}
