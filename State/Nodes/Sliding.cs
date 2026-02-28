using Godot;
using System;
using Id = SMNodeId;

public partial class Sliding : SMNode
{
    [Export] protected float Friction { get; set; } = 50;
    [Export] protected float CartwheelSpeed { get; set; } = 10;
    [Export] protected float MinimumSlideContinueDeg { get; set; } = 120;

    public Sliding() : base(Id.Sliding) { }

    public override void Enter(SMNode prevState)
    {
        fireExit(prevState.id);
        playAnimation();
    }

    public override void Exit(SMNode nextState)
    {
        fireExit(nextState.id);
    }

    public override Id? GetTransition()
    {
        var moveDir = getMoveDirection();
        var agent = getAgent();

        if (LabTools.VectorsWithinAngle(getVelocityH().Normalized(), moveDir.Normalized(), Mathf.DegToRad(MinimumSlideContinueDeg)))
        {
            return Id.Running;
        }
        else if (wantsJump())
        {
            if (moveDir != Vector3.Zero)
            {
                agent.Velocity = moveDir * CartwheelSpeed;
            }

            return Id.Jumping;
        }
        else if (getSpeedH() == 0)
        {
            return Id.Idle;
        }
        else if (!agent.IsOnFloor())
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

        var deltaV = Friction * (float)delta;
        var newVel = getVelocityH().Normalized() * Math.Max(0, getSpeedH() - deltaV);

        agent.Velocity = newVel + getVelocityV();

        agent.MoveAndSlide();
    }
}
