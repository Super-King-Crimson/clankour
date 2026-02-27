using Godot;
using System;
using Id = StateMachineNodeId;

public partial class State : Node
{
    [Export] protected StateMachine _stateMachine = null!;
    [Export] protected StateDetails _details = null!;

    [Export] protected StateMachineNode _startingState = null!;

    public override void _Ready()
    {
        _stateMachine.AddChildStates(this);
        _stateMachine.Init(_startingState);
    }

    public Node3D? Character { get => _details.Character; }

    public Vector3 GetVelocityH() => _details.GetVelocityH();
    public float GetSpeedH() => _details.GetSpeedH();
    public Vector3 GetVelocityV() => _details.GetVelocityV();
    public float GetSpeedV() => _details.GetSpeedV();
    public Vector3 GetPosition() => _details.GetPosition();
    public Transform3D GetTransform() => _details.GetTransform();

    public void ProcessPhysics(double delta) => _stateMachine.ProcessPhysics(delta);
    public void ProcessInput(InputEvent e) => _stateMachine.ProcessInput(e);

    public StateChangeResult TryChangeState(Id id) => _stateMachine.TryChangeState(id);
    public void ChangeState(Id id) => _stateMachine.ChangeState(id);
}
