using Godot;
using System;
using Id = SMNodeId;

public partial class Running : SMNode
{
    [Export] protected override string AnimationName { get; set; } = "enter anim name or die";
    [Export] protected float AnimSpeedBase { get; set; } = 0.5f;
    [Export] protected float AnimSpeedScale { get; set; } = 0.05f;

    [Export] protected float Acceleration { get; set; } = 7;
    [Export] protected float RotationSpeed { get; set; } = 10;
    [Export] protected float MaximumNoSlideDeg { get; set; } = 120;
    [Export] protected float MaxSpeed { get; set; } = 30;

    public Running() : base(Id.Running) { }

    protected override float getAnimationSpeed()
    {
        return AnimSpeedBase + (getSpeedH() * AnimSpeedScale);
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
        var moveDirection = getMoveDirection();

        if (!getAgent().IsOnFloor())
        {
            return Id.Airborne;
        }
        else if (wantsJump())
        {
            return Id.Jumping;
        }
        else if (getSpeedH() < _details.RunSpeed)
        {
            return Id.Walking;
        }
        else if (moveDirection == Vector3.Zero)
        {
            return Id.Idle;
        }
        else if (!LabTools.VectorsWithinAngle(getVelocityH().Normalized(), moveDirection.Normalized(), Mathf.DegToRad(MaximumNoSlideDeg)))
        {
            return Id.Sliding;
        }
        else
        {
            return null;
        }
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
