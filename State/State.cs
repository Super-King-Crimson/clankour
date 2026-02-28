using Godot;
using System;
using System.Linq;
using Id = SMNodeId;
using System.Collections.Generic;

public partial class State : Details, Machine
{
    [Export] protected SMNode _currentState = null!;
    [Export] protected SMNode _startingState = null!;
    [Export] protected SMNode _noneState = null!;

    public Dictionary<Id, SMNode> Nodes { get; set; } = new();

    public override void _Ready()
    {
        _currentState = _noneState;
        this.Init();

    }

    public override void _PhysicsProcess(double delta)
    {
        transitionToNextState();
        _currentState.ProcessPhysics(delta);
    }

    protected void changeState(SMNode newState)
    {
        if (_currentState != _noneState)
        {
            _currentState.StateEnded -= ChangeState;
            _currentState.Exit(newState);
        }

        LabTools.Log($"{_currentState.id} -> {newState.id}");
        newState.Enter(_currentState);

        newState.StateEnded += ChangeState;

        _currentState = newState;
    }

    protected void transitionToNextState(int retries = 5)
    {
        int i = 0;
        int MAX_RETRIES = retries;

        while (true)
        {
            if (_currentState.GetTransition() is not Id id)
            {
                break;
            }

            this.ChangeState(id);

            i++;

            if (i >= MAX_RETRIES)
            {
                throw new Exception($"Cyclic state loop detected related to agent {Agent.Name}'s state {_currentState.id}");
            }

        }
    }

    public void Init()
    {
        AddChildStatesOfNode(this);

        foreach (var (_, s) in this.GetStates())
        {
            LabTools.Log($"Added {s.id.ToString()}");
        }

        ChangeState(_startingState.id);
    }

    public bool TryChangeState(Id id)
    {
        if (!Nodes.TryGetValue(id, out var node)) return false;

        changeState(node);
        return true;
    }

    public void ChangeState(SMNodeId id)
    {
        if (TryChangeState(id) == false) throw new Exception($"Couldn't find state {id.ToString()}");
    }

    public void AddState(SMNode s)
    {
        s.Init(this);
        Nodes.Add(s.id, s);
    }

    public bool TryAddState(SMNode s)
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
            if (n is SMNode s)
            {
                this.TryAddState(s);
            }

            if (recursive)
            {
                this.AddChildStatesOfNode(n, true);
            }
        }

    }

    public bool TryRemoveState(Id id)
    {
        return Nodes.Remove(id);
    }

    public Dictionary<SMNodeId, SMNode> GetStates()
    {
        return Nodes;
    }
}
