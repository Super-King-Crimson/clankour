using Godot;
using System;
using System.Collections.Generic;

public interface StateMachine
{
    public StateMachineNode StartingState { get; protected set; }
    public StateMachineNode CurrentState { get; protected set; }
    public StateMachineNode NoneState { get; protected set; }
    public Dictionary<StateMachineNodeId, StateMachineNode> Nodes { get; protected set; }

    protected StateMachineDetails getDetails();

    protected void changeState(StateMachineNode newState)
    {
        if (CurrentState != NoneState)
        {
            CurrentState.StateEnded -= ChangeState;
            CurrentState.Exit(newState);
        }

        GD.Print($"{CurrentState.id} -> {newState.id}");
        newState.Enter(CurrentState);

        newState.StateEnded += ChangeState;

        CurrentState = newState;
    }

    protected void getNextValidState(int retries = 5)
    {
        int i = 0;
        int MAX_RETRIES = retries;

        while (true)
        {
            if (i >= MAX_RETRIES)
            {
                throw new Exception($"Cyclic state loop detected related to agent {getDetails().Agent.Name}'s state {CurrentState.id}");
            }

            if (!CurrentState.ExitIfInvalid())
            {
                break;
            }

            i++;
        }
    }

    public void AddState(StateMachineNode s)
    {
        s.Init(getDetails());
        Nodes.Add(s.id, s);
    }

    public bool TryAddState(StateMachineNode s)
    {
        try
        {
            this.AddState(s);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void AddChildStatesOfNode(Node parentNode, bool recursive = false)
    {
        if (parentNode.GetChildren().Count == 0) { return; }

        foreach (Node n in parentNode.GetChildren())
        {
            if (n is StateMachineNode s)
            {
                this.AddState(s);
            }

            if (recursive)
            {
                AddChildStatesOfNode(n, true);
            }
        }

    }

    public void TryAddChildStatesOfNode(Node parentNode, bool recursive = false)
    {
        if (parentNode.GetChildren().Count == 0) { return; }

        foreach (Node n in parentNode.GetChildren())
        {
            if (n is StateMachineNode s)
            {
                this.TryAddState(s);
            }

            if (recursive)
            {
                TryAddChildStatesOfNode(n, true);
            }
        }

    }

    public bool TryRemoveState(StateMachineNode s)
    {
        return Nodes.Remove(s.id);
    }

    public Dictionary<StateMachineNodeId, StateMachineNode> GetStates()
    {
        return Nodes;
    }

    public void ClearStates()
    {
        Nodes.Clear();
    }

    public void ChangeState(StateMachineNodeId id)
    {
        if (TryChangeState(id) is not StateChangeResult.Ok) throw new Exception($"Couldn't find state {id.ToString()}");
    }

    public StateChangeResult TryChangeState(StateMachineNodeId id)
    {
        if (!Nodes.TryGetValue(id, out var node)) return StateChangeResult.UnknownState;

        changeState(node);
        return StateChangeResult.Ok;
    }

    public void ProcessPhysics(double delta)
    {
        getNextValidState();
        CurrentState.ProcessPhysics(delta);
    }
}
