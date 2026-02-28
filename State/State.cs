using Godot;
using System;
using System.Linq;
using Id = StateMachineNodeId;
using System.Collections.Generic;

public partial class State : StateMachineDetails, StateMachine
{
    // IMPL StateMachine
    [Export] public StateMachineNode StartingState { get; set; } = null!;
    [Export] public StateMachineNode CurrentState { get; set; } = null!;
    [Export] public StateMachineNode NoneState { get; set; } = null!;

    public Dictionary<StateMachineNodeId, StateMachineNode> Nodes { get; set; } = new();

    public StateMachine machine;

    public State()
    {
        this.machine = (StateMachine)this;
    }

    public StateMachineDetails getDetails()
    {
        return this;
    }

    public override void _Ready()
    {
        var self = this.machine;

        CurrentState = NoneState;

        self.TryAddChildStatesOfNode(this);
        self.GetStates().Select(kv => kv.Value).ToList().ForEach(s => GD.Print(s.id));
        self.ChangeState(StartingState.id);
    }
}
