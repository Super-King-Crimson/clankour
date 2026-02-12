using Godot;
using System;

public partial class Jumping : MovementState3D
{
    [Export] private MovementState3D _airborneState;

    [Export] public int midairJumps = 0;
    [Export] public float jumpVelocity = 4.5f;

    private string _defaultAnimationName = "";
    [Export] public string cartwheelAnimationName;

    private bool _doingCartwheel = false;
    private bool _jumped = false;

    [Export] public float cartwheelJumpVelocity = 10.0f;

    public override State Enter(State prevState)
    {
        _jumped = false;

        if (prevState is Sliding)
        {
            _doingCartwheel = true;
            _defaultAnimationName = this.animationName;
            this.animationName = this.cartwheelAnimationName;
        }

        return base.Enter(prevState);
    }

    public override State Exit(State _)
    {
        if (_doingCartwheel)
        {
            this.animationName = _defaultAnimationName;
            _doingCartwheel = false;
        }

        return null;
    }

    public override State ProcessPhysics(double delta)
    {
        if (_jumped) return _airborneState;

        _jumped = true;

        var newVel = _agent.Velocity;

        float yVelocity = _doingCartwheel ? this.cartwheelJumpVelocity : this.jumpVelocity;

        newVel.Y = yVelocity;

        _agent.Velocity = newVel;

        return base.ProcessPhysics(delta);
    }

    public Jumping() : base("jumping") { }
}
