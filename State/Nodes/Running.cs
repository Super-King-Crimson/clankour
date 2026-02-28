using Godot;
using System;
using Id = StateMachineNodeId;

public partial class Running : StateMachineNode
{
    [Export] public override float AnimSpeedBase { get; set; } = 0.5f;
    [Export] public float AnimSpeedScale { get; set; } = 0.1f;

    [Export] public float Acceleration { get; set; } = 7;
    [Export] public float RotationSpeed { get; set; } = 1;
    [Export] public float MaximumNoSlideDeg { get; set; } = 120;
    [Export] public float MaxSpeed { get; set; } = 30;

    public Running() : base(Id.Running) { }

    protected override float getAnimationSpeed()
    {
        return AnimSpeedBase * getSpeedH() * AnimSpeedScale;
    }

    public override void Enter(StateMachineNode prevState)
    {
        playAnimation();
    }

    public override void Exit(StateMachineNode nextState) { }

    public override bool ExitIfInvalid()
    {
        var moveDirection = getMoveDirection();

        if (!getAgent().IsOnFloor())
        {
            fireExit(Id.Airborne);
            return true;
        }

        if (wantsJump())
        {
            fireExit(Id.Jumping);
            return true;
        }

        if (getSpeedH() < _details.RunSpeed)
        {
            fireExit(Id.Walking);
            return true;
        }

        if (moveDirection == Vector3.Zero)
        {
            fireExit(Id.Idle);
            return true;
        }

        if (!LabTools.VectorsWithinAngle(getVelocityH().Normalized(), moveDirection.Normalized(), Mathf.DegToRad(MaximumNoSlideDeg)))
        {
            fireExit(Id.Sliding);
            return true;
        }

        return false;
    }

    public override void ProcessPhysics(double delta)
    {
        var agent = getAgent();
        Vector3 direction = getMoveDirection();
        Vector3 prevVel = getVelocityH();

        float fdelta = (float)delta;
        float deltaV = Acceleration * fdelta;

        float speed = Math.Min(MaxSpeed, getSpeedH() + deltaV);

        var newVel = rotate(prevVel.Normalized(), direction.Normalized(), RotationSpeed * fdelta) * speed;
        newVel += getVelocityV();

        agent.Velocity = newVel;

        if (getCharacter() is Node3D c)
        {
            c.LookAt(agent.Position + agent.Velocity);
        }

        setAnimationSpeed();
        agent.MoveAndSlide();
    }
}
