using Godot;
using System;
using Id = SMNodeId;

public partial class None : SMNode
{
    public None() : base(Id.None) { }

    public override void Enter(SMNode prevState)
    {
        fireEnter(prevState.id);
    }

    public override void Exit(SMNode nextState)
    {
        fireExit(nextState.id);
    }

    public override Id? GetTransition() => null;
    public override void ProcessPhysics(double delta) { }
}
