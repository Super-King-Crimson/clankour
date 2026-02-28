using Godot;
using System;
using Id = SMNodeId;

public enum SMNodeId
{
    None,
    Airborne,
    Idle,
    Jumping,
    Running,
    Sliding,
    Walking
}

public abstract partial class SMNode : Node
{
    public readonly SMNodeId id;
    protected Details _details = null!;

    [Signal] public delegate void StateEnteredEventHandler(SMNodeId newStateId);
    [Signal] public delegate void StateEndedEventHandler(SMNodeId newStateId);

    [Export] protected virtual string AnimationName { get; set; } = "ENTER ANIMATION NAME";

    public SMNode(Id id)
    {
        this.id = id;
    }

    public abstract void Enter(SMNode prevState);
    public abstract void Exit(SMNode nextState);
    public abstract Id? GetTransition();
    public abstract void ProcessPhysics(double delta);

    protected virtual void fireEnter(SMNodeId prevStateId)
    {
        this.EmitSignal(SignalName.StateEntered, (int)prevStateId);
    }

    protected void fireExit(SMNodeId nextStateId)
    {
        this.EmitSignal(SignalName.StateEnded, (int)nextStateId);
    }

    protected virtual float getAnimationSpeed() => 1;
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
        float radBetween = fromVec.AngleTo(toVec);

        Vector3 goalRot = toVec;

        if (radBetween > rad)
        {
            // if degBetween is nearly 180 (radBetween = pi), game doesn't know where to rotate
            if (Mathf.IsEqualApprox(radBetween, Mathf.Pi))
            {
                // always rotate consistently, use character vs agent transform as tie breaker
                // we always want to nudge inwards wrt camera, and camera is locked to agent
                // what is inwards defined mathematically?
                // we can draw a bird's eye view of player to make problem 2D (we're rotating based on TransformY)
                // then we can draw rotation arrows based on the direction we want to rotate a bunch of arbitrary vectors
                // see explain.png, and see how everything converges on the line of y = -x 
                // going back to 3D, we can always get TransformX and Z, which map to local right and back respectively
                // so if we just combine these two vectors and normalize them, we get the direction we want to rotate
                // BUT WHAT IF WE'RE FACING THAT DIRECTION? (OR THE DIRECT OPPOSITE DIRECTION???)
                // Calm down. Consult the graph again.
                // Imagine what you would expect to happen if you were directly on that line.
                // You'd rotate towards y = x (local right and forward) right? Ok cool. Now code the nudge.
                // Man, if only we had a function that allowed us to rotate some angle between two vectors...
                var agentBasis = getAgent().Transform.Basis;
                var axis = agentBasis.Y;
                var goalVec = (agentBasis.X + agentBasis.Z).Normalized();

                // if facing goal vector
                if (Mathf.IsEqualApprox(Mathf.Abs(fromVec.Normalized().Dot(goalVec)), 1))
                {
                    goalVec.Z = -1 * goalVec.Z;
                }

                // oh wait. that's THIS function!
                // we're only multiplying by 2 here to COMPLETELY ENSURE we're out of the range and won't have a recursive call
                // We change the goalVec if absolute of the dot was EqualApprox to 1 (which uses Epsilon),
                // so using this we can ensure we bypass that check
                fromVec = rotate(fromVec, goalVec, 2 * Mathf.Epsilon);
            }

            // now code the whole move, which will be encoded properly!
            goalRot = fromVec.Slerp(toVec, rad / radBetween);
        }

        return goalRot;
    }

    protected Vector3 getVelocityH() => _details.GetVelocityH();
    protected float getSpeedH() => _details.GetSpeedH();

    protected Vector3 getVelocityV() => _details.GetVelocityV();
    protected float getSpeedV() => _details.GetSpeedV();

    protected Vector3 getPosition() => _details.GetPosition();
    protected Transform3D getTransform() => _details.GetTransform();

    public virtual void Init(Details details)
    {
        _details = details;
    }
}
