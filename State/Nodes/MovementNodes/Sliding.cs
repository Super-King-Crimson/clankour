using Godot;
using System;
using Id = StateMachineNodeId;

public partial class Sliding : Movement3DNode
{
    [Export] public float Friction { get; set; } = 50;
    [Export] public float CartwheelSpeed { get; set; } = 10;
    [Export] public float MinimumSlideContinueDeg { get; set; } = 120;

    public Sliding() : base(Id.Sliding) { }

    public override bool ExitIfInvalid()
    {
        var moveDir = getMoveDirection();
        var agent = getAgent();

        if (LabTools.VectorsWithinAngle(getVelocityH().Normalized(), moveDir.Normalized(), Mathf.DegToRad(MinimumSlideContinueDeg)))
        {
            fireExit(Id.Running);
            return true;
        }

        if (wantsJump())
        {
            if (moveDir != Vector3.Zero)
            {
                agent.Velocity = moveDir * CartwheelSpeed;
            }

            fireExit(Id.Jumping);
            return true;
        }

        if (getSpeedH() == 0)
        {
            fireExit(Id.Idle);
            return true;
        }

        if (!agent.IsOnFloor())
        {
            fireExit(Id.Airborne);
            return true;
        }

        return false;
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
