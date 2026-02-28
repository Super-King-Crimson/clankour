using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GDC = Godot.Collections;
using Id = StateMachineNodeId;

public partial class Airborne : StateMachineNode
{
    [Export] public float Acceleration { get; set; } = 3;
    [Export] public float MaxSpeed { get; set; } = 20;
    [Export] public float RotationSpeed { get; set; } = 5;

    [Export] public float MaxNoDecelerateDeg { get; set; } = 90;
    [Export] public float CoyoteTime { get; set; } = 0.25f;

    [Export] public GDC.Array<StateMachineNodeId> CanCoyoteFromStates { get; set; } = new();

    private float _coyoteTimer = 0;
    private bool _connected = false;

    public Airborne() : base(Id.Airborne) { }

    private void playOnAnimationFinished(StringName _) => playAnimation();

    private bool canCoyote() => _coyoteTimer < CoyoteTime;

    public override void Enter(StateMachineNode prevState)
    {
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

    public override void Exit(StateMachineNode nextState)
    {
        if (!_connected) return;

        _connected = false;
        getAnimator().AnimationFinished -= playOnAnimationFinished;
    }

    public override bool ExitIfInvalid()
    {
        if (getAgent().IsOnFloor())
        {
            fireExit(Id.Idle);
            return true;
        }

        if (wantsJump() && canCoyote())
        {
            fireExit(Id.Jumping);
            return true;
        }

        return false;
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
