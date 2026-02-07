using Godot;
using System;

public partial class StateMachine : Node
{
    [Export] public MovementState3D startingState;

    private IMover _mover;
    private MovementState3D _currentState;

    public void Init(CharacterBody3D character, AnimationPlayer animator)
    {

        foreach (var child in GetChildren())
        {
            GD.Print($"{child.Name}");
            if (child is MovementState3D state)
            {
                state.Init(character, animator, _mover);
                GD.Print($"Initialized {state.Name}");
            }

        }
    }

    public void ChangeState(MovementState3D newState)
    {
        if (_currentState is not null)
            _currentState.Exit();

        _currentState = newState;
        _currentState.Enter();
    }

    public void ProcessFrame(float delta)
    {
        MovementState3D newState = _currentState.ProcessFrame(delta);

        if (newState is not null)
            this.ChangeState(newState);
    }

    public void ProcessInput(InputEvent e)
    {
        MovementState3D newState = _currentState.ProcessInput(e);

        if (newState is not null)
            this.ChangeState(newState);
    }

    public void ProcessPhysics(float delta)
    {
        MovementState3D newState = _currentState.ProcessPhysics(delta);

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
