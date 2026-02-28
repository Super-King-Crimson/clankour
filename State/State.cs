using Godot;
using System;
using System.Linq;
using Id = StateMachineNodeId;

public partial class State : Node
{
    [Export] protected StateMachine _stateMachine = null!;
    [Export] protected StateMachineNode _startingState = null!;
    [Export] protected StateMachineNode _noneState = null!;

    [Export] protected StateMachineDetails _details = null!;
    [Export] protected CharacterBody3D _agent = null!;
    [Export] protected AnimationPlayer _animator = null!;
    [Export] protected Mover _mover = null!;
    [Export] protected Node3D _character = null!;

    public Node3D Character { get => _details.Character; }

    public override void _Ready()
    {
        _details.Init(_agent, _animator, _mover, _character);

        _stateMachine.Init(_startingState, _details, _noneState);

        _stateMachine.TryAddChildStatesOfNode(this);
        _stateMachine.GetStates().Select(kv => kv.Value).ToList().ForEach(s => GD.Print(s.id));
        _stateMachine.ChangeState(_startingState.id);
    }

    public Vector3 GetVelocityH() => _details.GetVelocityH();
    public float GetSpeedH() => _details.GetSpeedH();
    public Vector3 GetVelocityV() => _details.GetVelocityV();
    public float GetSpeedV() => _details.GetSpeedV();
    public Vector3 GetPosition() => _details.GetPosition();
    public Transform3D GetTransform() => _details.GetTransform();

    public void ProcessPhysics(double delta) => _stateMachine.ProcessPhysics(delta);

    public StateChangeResult TryChangeState(Id id) => _stateMachine.TryChangeState(id);
    public void ChangeState(Id id) => _stateMachine.ChangeState(id);
}
