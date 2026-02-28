using Godot;
using System;
using Id = SMNodeId;

public partial class Idle : SMNode
{
    [Export] protected float Friction { get; set; } = 30;

    public Idle() : base(Id.Idle) { }

    public override void Enter(SMNode prevState)
    {
        fireEnter(prevState.id);
        playAnimation();
    }

    public override void Exit(SMNode nextState)
    {
        fireExit(nextState.id);
    }

    public override Id? GetTransition()
    {
        if (getMoveDirection() != Vector3.Zero)
        {
            return Id.Walking;
        }
        else if (wantsJump())
        {
            return Id.Jumping;
        }
        else if (!getAgent().IsOnFloor())
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

        Vector3 prevVel = agent.Velocity;
        var speed = prevVel.Length();

        float deltaV = Friction * (float)delta;
        float newSpeed = Math.Max(0, speed - deltaV);

        agent.Velocity = prevVel.Normalized() * newSpeed;

        agent.MoveAndSlide();
    }
}
