using Godot;
using System;
using Id = StateMachineNodeId;

public partial class Walking : Movement3DNode
{
    [Export] public float Acceleration { get; set; } = 3;
    [Export] public float MinSpeed { get; set; } = 2;
    [Export] public float RotationSpeed { get; set; } = 8;

    [Export] public override float AnimSpeedBase { get; set; } = 0.5f;
    [Export] public float AnimSpeedScale { get; set; } = 1;

    [Export] public float MaxAccelerationDeg { get; set; } = 50;

    public Walking() : base(Id.Walking) { }

    protected override float getAnimationSpeed()
    {
        return AnimSpeedBase + (AnimSpeedScale * (getSpeedH() / _details.RunSpeed));
    }

    public override bool ExitIfInvalid()
    {
        var agent = getAgent();

        if (wantsJump())
        {
            fireExit(Id.Idle);
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

        if (LabTools.VectorsWithinAngle(prevVelNorm, directionNorm, MaxAccelerationDeg) && newSpeed > MinSpeed)
        {
            newSpeed += deltaV;
        }
        else
        {
            newSpeed = Math.Max(MinSpeed, newSpeed + deltaV);
        }

        var newVel = rotate(prevVelNorm, directionNorm, fdelta * RotationSpeed) * newSpeed;
        newVel += getVelocityV();
        agent.Velocity = newVel;

        if (getCharacter() is CharacterBody3D c)
        {
            c.LookAt(agent.Position + agent.Velocity);
        }

        setAnimationSpeed();
        agent.MoveAndSlide();
    }
}

