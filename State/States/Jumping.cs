using Godot;
using System;

public partial class Jumping : MovementState3D
{
    [Export] public MovementState3D airborneState;

    [Export] public int MidairJumps { get; set; } = 1;
    [Export] public float JumpVelocity { get; set; } = 4.5f;

    public float cartwheelBoost = 3.0f;

    private bool _doingCartwheel = false;

    public override State Enter(State prevState)
    {
        if (prevState == airborneState)
            if (MidairJumps <= 0)
                return prevState;

        if (prevState is Sliding)
        {
            _doingCartwheel = true;
            JumpVelocity += this.cartwheelBoost;
        }

        return base.Enter(prevState);
    }

    public override State Exit(State _)
    {
        if (_doingCartwheel)
        {
            _doingCartwheel = false;
            JumpVelocity -= this.cartwheelBoost;
        }

        return null;
    }

    public override MovementState3D ProcessPhysics(double delta)
    {
        var newVel = _agent.Velocity;
        newVel.Y = JumpVelocity;

        _agent.Velocity = newVel;
        base.ProcessPhysics(delta);

        return airborneState;
    }
}
