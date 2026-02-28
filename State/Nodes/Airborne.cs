using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GDC = Godot.Collections;
using Id = SMNodeId;

public partial class Airborne : SMNode
{
    [Export] protected float Acceleration { get; set; } = 3;
    [Export] protected float MaxSpeed { get; set; } = 20;
    [Export] protected float RotationSpeed { get; set; } = 5;

    [Export] protected float MaxNoDecelerateDeg { get; set; } = 90;
    [Export] protected float CoyoteTime { get; set; } = 0.25f;

    [Export] protected GDC.Array<SMNodeId> CanCoyoteFromStates { get; set; } = new();

    private float _coyoteTimer = 0;
    private bool _connected = false;

    public Airborne() : base(Id.Airborne) { }

    private void playOnAnimationFinished(StringName _) => playAnimation();

    private bool canCoyote() => _coyoteTimer < CoyoteTime;

    public override void Enter(SMNode prevState)
    {
        fireEnter(prevState.id);

        if (CanCoyoteFromStates.Contains(prevState.id))
        {
            _coyoteTimer = 0;
            playAnimation();
        }
        else
        {
            _coyoteTimer = CoyoteTime;
            _connected = true;

            getAnimator().AnimationFinished += playOnAnimationFinished;
        }
    }

    public override void Exit(SMNode nextState)
    {
        fireExit(nextState.id);
        if (!_connected) return;

        _connected = false;
        getAnimator().AnimationFinished -= playOnAnimationFinished;
    }

    public override Id? GetTransition()
    {
        if (getAgent().IsOnFloor())
        {
            return Id.Idle;
        }
        else if (wantsJump() && canCoyote())
        {
            return Id.Jumping;
        }
        else
        {
            return null;
        }
    }

    public override void ProcessPhysics(double delta)
    {
        float fdelta = (float)delta;
        _coyoteTimer += fdelta;

        CharacterBody3D agent = getAgent();
        Vector3 directionNorm = getMoveDirection().Normalized();

        if (directionNorm != Vector3.Zero)
        {
            var velH = getVelocityH();
            var speed = getSpeedH();

            // change directionXZNorm to direction you should face if no velocity
            var velHNorm = speed == 0 ?
                directionNorm : velH.Normalized();

            float deltaV = Acceleration * fdelta;

            Vector3 newVel = velHNorm;
            if (speed != 0 && !LabTools.VectorsWithinAngle(velHNorm, directionNorm, Mathf.DegToRad(MaxNoDecelerateDeg)))
            {
                speed -= deltaV;
            }
            else
            {
                speed = Math.Min(MaxSpeed, speed + deltaV);
                newVel = rotate(newVel, directionNorm, fdelta * RotationSpeed);
            }

            newVel *= speed;
            agent.Velocity = newVel + getVelocityV();

            if (getCharacter() is Node3D c)
            {
                c.LookAt(agent.Position + rotate(-1 * c.GlobalTransform.Basis.Z, directionNorm, fdelta * RotationSpeed));
            }
        }

        agent.Velocity += agent.GetGravity() * fdelta;
        agent.MoveAndSlide();
    }
}
