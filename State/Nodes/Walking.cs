using Godot;
using System;
using Id = StateMachineNodeId;

public partial class Walking : StateMachineNode
{
    [Export] public float Acceleration { get; set; } = 3;
    [Export] public float MinSpeed { get; set; } = 2;
    [Export] public float RotationSpeed { get; set; } = 20;

    [Export] public override string AnimationName { get; set; } = null!;
    [Export] public float AnimSpeedBase { get; set; } = 0.3f;
    [Export] public float AnimSpeedScale { get; set; } = 1.3f;

    [Export] public float MaxAccelerationDeg { get; set; } = 10;

    public Walking() : base(Id.Walking) { }

    protected override float getAnimationSpeed()
    {
        return AnimSpeedBase + (AnimSpeedScale * (getSpeedH() / _details.RunSpeed));
    }

    public override void Enter(StateMachineNode prevState)
    {
        playAnimation();
    }

    public override void Exit(StateMachineNode nextState) { }

    public override bool ExitIfInvalid()
    {
        var agent = getAgent();

        if (wantsJump())
        {
            fireExit(Id.Jumping);
            return true;
        }

        if (getMoveDirection() == Vector3.Zero)
        {
            fireExit(Id.Idle);
            return true;
        }

        if (getSpeedH() > _details.RunSpeed)
        {
            fireExit(Id.Running);
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

        Vector3 prevVelNorm = getVelocityH().Normalized();
        Vector3 directionNorm = getMoveDirection().Normalized();

        float newSpeed = getSpeedH();
        float fdelta = (float)delta;
        float deltaV = fdelta * Acceleration;

        var newVelNorm = rotate(prevVelNorm, directionNorm, fdelta * RotationSpeed);

        if (LabTools.VectorsWithinAngle(prevVelNorm, newVelNorm, Mathf.DegToRad(MaxAccelerationDeg)) && newSpeed >= MinSpeed)
        {
            newSpeed += deltaV;
        }
        else
        {
            newSpeed = Math.Min(MinSpeed, newSpeed + deltaV);
        }

        agent.Velocity = newVelNorm * newSpeed + getVelocityV();

        if (getCharacter() is Node3D n)
        {
            n.LookAt(agent.Position + agent.Velocity);
        }

        setAnimationSpeed();
        agent.MoveAndSlide();
    }
}

