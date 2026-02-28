using Godot;
using System;
using Id = StateMachineNodeId;

public partial class Idle : StateMachineNode
{
    [Export] public float Friction { get; set; } = 30;

    public Idle() : base(Id.Idle) { }

    public override void Enter(StateMachineNode prevState)
    {
        playAnimation();
    }

    public override void Exit(StateMachineNode nextState) { }

    public override bool ExitIfInvalid()
    {
        if (getMoveDirection() != Vector3.Zero)
        {
            fireExit(Id.Walking);
            return true;
        }

        if (wantsJump())
        {
            fireExit(Id.Jumping);
            return true;
        }

        if (!getAgent().IsOnFloor())
        {
            fireExit(Id.Airborne);
            return true;
        }

        return false;
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
