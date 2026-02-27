using Godot;
using System;
using static StateMachineNodeId;

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
    InvalidPlayerState = 101,
    InvalidStateTransition = 102,
    UnknownState = 103,
}

public partial class StateMachineNode : Node
{
    public static readonly StateMachineNode VOID = new StateMachineNode();

    public readonly StateMachineNodeId id;

    [Signal]
    public delegate void StateEndedEventHandler(StateMachineNodeId newStateId);

    public StateMachineNode() => this.id = None;

    protected StateMachineNode(StateMachineNodeId id) => this.id = id;

    protected void fireExit(StateMachineNodeId nextStateId)
    {
        this.EmitSignal(SignalName.StateEnded, (int)nextStateId);
    }

    public virtual bool CanTransition(StateMachineNode state) => true;
    public virtual bool ExitIfInvalid() => false;

    public virtual void Enter(StateMachineNode prevState) => GD.Print("Entering void state");
    public virtual void Exit(StateMachineNode nextState) => GD.Print("Exiting void state");

    public virtual void ProcessFrame(double delta) { }
    public virtual void ProcessInput(InputEvent e) { }
    public virtual void ProcessPhysics(double delta) { }
}
