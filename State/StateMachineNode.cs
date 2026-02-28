using Godot;
using System;
using Id = StateMachineNodeId;

public enum StateMachineNodeId
{
    None,
    Airborne,
    Idle,
    Jumping,
    Running,
    Sliding,
    Walking
}

public enum StateChangeResult
{
    Ok = 0,
    UnknownState = 101,
}

public abstract partial class StateMachineNode : Node
{
    public readonly StateMachineNodeId id;
    protected StateMachineDetails _details = null!;

    [Signal] public delegate void StateEnteredEventHandler(StateMachineNodeId newStateId);
    [Signal] public delegate void StateEndedEventHandler(StateMachineNodeId newStateId);

    [Export] public virtual float AnimSpeedBase { get; set; } = 1;
    [Export] public virtual string AnimationName { get; set; } = "";

    public StateMachineNode(StateMachineNodeId id = Id.None)
    {
        this.id = id;
    }

    public abstract void Enter(StateMachineNode prevState);
    public abstract void Exit(StateMachineNode nextState);
    public abstract bool ExitIfInvalid();
    public abstract void ProcessPhysics(double delta);

    public virtual void Init(StateMachineDetails details)
    {
        _details = details;
    }

    protected virtual void fireEnter(StateMachineNodeId prevStateId)
    {
        this.EmitSignal(SignalName.StateEntered, (int)prevStateId);
    }

    protected void fireExit(StateMachineNodeId nextStateId)
    {
        this.EmitSignal(SignalName.StateEnded, (int)nextStateId);
    }

    protected virtual float getAnimationSpeed() => AnimSpeedBase;
    protected virtual void setAnimationSpeed() => getAnimator().SpeedScale = getAnimationSpeed();

    protected virtual void playAnimation()
    {
        setAnimationSpeed();
        _details.Animator.Play(AnimationName);
    }

    protected CharacterBody3D getAgent() => _details.Agent;
    protected AnimationPlayer getAnimator() => _details.Animator;
    protected Node3D getCharacter() => _details.Character;

    protected Vector3 getMoveDirection() => _details.Mover.GetInputDirectionRelative(getAgent().Transform);
    protected bool wantsJump() => _details.Mover.WantsJump();

    protected Vector3 rotate(Vector3 fromVec, Vector3 toVec, float rad)
    {
        float degBetween = fromVec.AngleTo(toVec);

        Vector3 goalRot = toVec;

        if (degBetween > rad)
        {
            // HACK: if degBetween is exactly 180 (radBetween = pi), game doesn't know where to rotate
            // to fix, rotate by small small fraction of a rad on agent's y transform
            // so it can correctly guess the direction in which to rotate
            if (degBetween - Mathf.Pi <= Mathf.Epsilon) fromVec = fromVec.Rotated(getAgent().Transform.Basis.Y, Mathf.Epsilon);

            goalRot = fromVec.Slerp(toVec, rad / degBetween);
        }

        return goalRot;
    }

    protected Vector3 getVelocityH() => _details.GetVelocityH();
    protected float getSpeedH() => _details.GetSpeedH();

    protected Vector3 getVelocityV() => _details.GetVelocityV();
    protected float getSpeedV() => _details.GetSpeedV();

    protected Vector3 getPosition() => _details.GetPosition();
    protected Transform3D getTransform() => _details.GetTransform();
}
