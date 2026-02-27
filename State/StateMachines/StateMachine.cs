using Godot;
using System;
using System.Collections.Generic;
using static StateMachineNode;

public partial class StateMachine : Node
{
    public StateMachineNode StartingState { get; protected set; } = StateMachineNode.VOID;

    protected Dictionary<StateMachineNodeId, StateMachineNode> _nodes = new();
    protected StateDetails _details = null!;

    protected StateMachineNode _currentState = VOID;

    public void Init(StateMachineNode startingState)
    {
        StartingState = startingState;
        changeState(StartingState);
    }

    protected void changeState(StateMachineNode newState)
    {
        if (_currentState != VOID)
        {
            _currentState.StateEnded -= ChangeState;
        }

        _currentState.Exit(newState);
        newState.Enter(_currentState);

        newState.StateEnded += ChangeState;

        _currentState = newState;
    }

    protected void getNextValidState(int retries = 3)
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
        _nodes.Add(s.id, s);
    }

    public bool TryAddState(StateMachineNode s)
    {
        return _nodes.TryAdd(s.id, s);
    }

    public void AddChildStates(Node parentNode, bool recursive = false)
    {
        if (parentNode.GetChildren().Count == 0) { return; }

        foreach (Node n in parentNode.GetChildren())
        {
            if (n is StateMachineNode s)
            {
                TryAddState(s);
            }

        }

        if (recursive)
        {
            AddChildStates(parentNode, true);
        }
    }

    public bool TryRemoveState(StateMachineNode s)
    {
        return _nodes.Remove(s.id);
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

        if (!_currentState.CanTransition(node)) return StateChangeResult.InvalidStateTransition;

        changeState(node);
        return StateChangeResult.Ok;
    }

    public void ProcessFrame(double delta)
    {
        getNextValidState();
        _currentState.ProcessFrame(delta);
    }

    public void ProcessPhysics(double delta)
    {
        getNextValidState();
        _currentState.ProcessPhysics(delta);
    }

    public void ProcessInput(InputEvent e)
    {
        getNextValidState();
        _currentState.ProcessInput(e);
    }
}
