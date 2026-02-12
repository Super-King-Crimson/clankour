using Godot;
using System;

public partial class StateMachine : Node
{
    [Export] public State startingState;

    private Node3D _character;
    private State _currentState;
    private IMover _mover;

    public void Init(CharacterBody3D agent, AnimationPlayer animator)
    {
        foreach (var child in GetChildren())
        {
            if (child is MovementState3D state)
            {
                GD.Print(state.id);
                state.Init(agent, animator, _mover);

                if (_character is not null)
                {
                    state.SetCharacter(_character);
                }
            }
        }

        this.ChangeState(this.startingState);
    }

    public void SetCharacter(Node3D character)
    {
        _character = character;
    }

    public void ChangeState(State state)
    {
        if (state is null)
        {
            throw new Exception("Cannot set state to null");
        }

        State newState = state;
        while (newState != null)
        {
            if (_currentState is not null)
                _currentState.Exit(state);

            GD.Print($"Going from {_currentState?.id ?? "stateless"} to {newState.id}");
            State prevState = _currentState;
            _currentState = newState;

            newState = _currentState.Enter(prevState);
        }
    }

    public void ProcessFrame(double delta)
    {
        if (_currentState is null)
        {
            throw new Exception("Must initialize and set starting state of state machine");
        }

        State newState = _currentState.ProcessFrame(delta);
        while (newState != null)
        {
            this.ChangeState(newState);

            newState = _currentState.ProcessFrame(delta);
        }
    }

    public void ProcessInput(InputEvent e)
    {
        if (_currentState is null)
        {
            throw new Exception("Must initialize and set starting state of state machine");
        }

        State newState = _currentState.ProcessInput(e);
        while (newState != null)
        {
            this.ChangeState(newState);

            newState = _currentState.ProcessInput(e);
        }
    }

    public void ProcessPhysics(double delta)
    {
        if (_currentState is null)
        {
            throw new Exception("Must initialize and set starting state of state machine");
        }

        State newState = _currentState.ProcessPhysics(delta);
        while (newState != null)
        {
            this.ChangeState(newState);

            newState = _currentState.ProcessPhysics(delta);
        }
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
