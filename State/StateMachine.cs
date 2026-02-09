using Godot;
using System;

public partial class StateMachine : Node
{
    [Export] public State startingState;

    private IMover _mover;
    private State _currentState;

    public void Init(CharacterBody3D agent, AnimationPlayer animator)
    {
        foreach (var child in GetChildren())
        {
            if (child is MovementState3D state)
            {
                state.Init(agent, animator, _mover);
            }
        }

        _currentState = startingState;
        startingState.Enter(null);
    }

    public void ChangeState(State newState)
    {
        if (_currentState is not null)
            _currentState.Exit(newState);

        State temp = newState;
        while (temp != null)
        {
            GD.Print($"New state: {temp.Name}");
            temp = temp.Enter(_currentState);
        }

        _currentState = newState;
    }

    public void ProcessFrame(double delta)
    {
        State newState = _currentState.ProcessFrame(delta);

        if (newState is not null)
            this.ChangeState(newState);
    }

    public void ProcessInput(InputEvent e)
    {
        State newState = _currentState.ProcessInput(e);

        if (newState is not null)
            this.ChangeState(newState);
    }

    public void ProcessPhysics(double delta)
    {
        State newState = _currentState.ProcessPhysics(delta);

        if (newState is not null)
            this.ChangeState(newState);
    }

    public override void _Ready()
    {
        foreach (Node n in GetChildren())
        {
            if (n is IMover mover)
            {
                _mover = mover;
                break;
            }
        }

        if (_mover is null)
            throw new Exception("StateMachine that controls movement must have a child node that implements IMover");
    }
}
