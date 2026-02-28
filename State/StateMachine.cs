using Godot;
using System;
using System.Collections.Generic;
using static StateMachineNode;

public partial class StateMachine : Node
{
    protected StateMachineNode _noneState = null!;

    protected StateMachineNode _startingState = null!;
    protected StateMachineNode _currentState = null!;
    protected StateMachineDetails _details = null!;
    protected Dictionary<StateMachineNodeId, StateMachineNode> _nodes = new();

    public bool IsInitialized { get; protected set; } = false;

    public void Init(StateMachineNode startingState, StateMachineDetails details, StateMachineNode noneState)
    {
        if (IsInitialized) GD.PrintErr("StateMachine initialized more than once, is this error?");

        IsInitialized = true;

        _noneState = noneState;

        _startingState = startingState;
        _details = details;
        _currentState = _noneState;
    }

    protected void changeState(StateMachineNode newState)
    {
        if (_currentState != _noneState)
        {
            _currentState.StateEnded -= ChangeState;
            _currentState.Exit(newState);
        }

        GD.Print($"{_currentState.id} -> {newState.id}");
        newState.Enter(_currentState);

        newState.StateEnded += ChangeState;

        _currentState = newState;
    }

    protected void getNextValidState(int retries = 5)
    {
        int i = 0;
        int MAX_RETRIES = retries;

        while (true)
        {
            if (i >= MAX_RETRIES)
            {
                throw new Exception($"Cyclic state loop detected related to agent {_details.Agent.Name}'s state {_currentState.id}");
            }

            if (!_currentState.ExitIfInvalid())
            {
                break;
            }

            i++;
        }
    }

    public void AddState(StateMachineNode s)
    {
        s.Init(_details);
        _nodes.Add(s.id, s);
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
        return _nodes.Remove(s.id);
    }

    public Dictionary<StateMachineNodeId, StateMachineNode> GetStates()
    {
        return _nodes;
    }

    public void ClearStates()
    {
        _nodes.Clear();
    }

    public void ChangeState(StateMachineNodeId id)
    {
        if (TryChangeState(id) is not StateChangeResult.Ok) throw new Exception($"Couldn't find state {id.ToString()}");
    }

    public StateChangeResult TryChangeState(StateMachineNodeId id)
    {
        if (!_nodes.TryGetValue(id, out var node)) return StateChangeResult.UnknownState;

        changeState(node);
        return StateChangeResult.Ok;
    }

    public void ProcessPhysics(double delta)
    {
        getNextValidState();
        _currentState.ProcessPhysics(delta);
    }
}
