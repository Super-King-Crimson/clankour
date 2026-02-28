using Godot;
using System;
using Id = SMNodeId;

public partial class Walking : SMNode
{
    [Export] protected float Acceleration { get; set; } = 3;
    [Export] protected float MinSpeed { get; set; } = 2;
    [Export] protected float RotationSpeed { get; set; } = 20;

    [Export] protected override string AnimationName { get; set; } = null!;
    [Export] protected float AnimSpeedBase { get; set; } = 0.3f;
    [Export] protected float AnimSpeedScale { get; set; } = 1.3f;

    [Export] public float MaxAccelerationDeg { get; set; } = 10;

    public Walking() : base(Id.Walking) { }

    protected override float getAnimationSpeed()
    {
        return AnimSpeedBase + (AnimSpeedScale * (getSpeedH() / _details.RunSpeed));
    }

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
        var agent = getAgent();

        if (wantsJump())
        {
            return Id.Jumping;
        }
        else if (getMoveDirection() == Vector3.Zero)
        {
            return Id.Idle;
        }
        else if (getSpeedH() > _details.RunSpeed)
        {
            return Id.Running;
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

