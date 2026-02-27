using Godot;
using System;
using static LabTools;

public abstract partial class Movement3DNode : StateMachineNode
{
    [Export] public virtual string AnimationName { get; set; } = null!;
    [Export] public virtual float AnimSpeedBase { get; set; } = 1;

    protected StateDetails _details = null!;

    public Movement3DNode(StateMachineNodeId id) : base(id) { }
    public override void Enter(StateMachineNode prevState) { }
    public override void Exit(StateMachineNode nextState) { }

    protected virtual float getAnimationSpeed() => AnimSpeedBase;
    protected void setAnimationSpeed() => _details.Animator.SpeedScale = getAnimationSpeed();

    protected virtual void playAnimation()
    {
        setAnimationSpeed();
        _details.Animator.Play(AnimationName);
    }

    protected CharacterBody3D getAgent() => _details.Agent;
    protected AnimationPlayer getAnimator() => _details.Animator;
    protected Node? getCharacter() => _details.Character;

    protected Vector3 getMoveDirection() => _details.Mover.GetInputDirectionRelative(getAgent().Transform);
    protected bool wantsJump() => _details.Mover.WantsJump();

    public Vector3 getVelocityH() => _details.GetVelocityH();
    public float getSpeedH() => _details.GetSpeedH();

    public Vector3 getVelocityV() => _details.GetVelocityV();
    public float getSpeedV() => _details.GetSpeedV();

    public Vector3 getPosition() => _details.GetPosition();
    public Transform3D getTransform() => _details.GetTransform();

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
}
