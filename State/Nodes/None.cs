using Godot;
using System;
using Id = StateMachineNodeId;

public partial class None : StateMachineNode
{
    public None() : base() { }
    protected None(Id id) : base(id) { }

    public override void Enter(StateMachineNode prevState) { }
    public override void Exit(StateMachineNode nextState) { }
    public override bool ExitIfInvalid() => false;
    public override void ProcessPhysics(double delta) { }
}
