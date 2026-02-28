using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [Export] protected State _state = null!;

    public override void _PhysicsProcess(double delta)
    {
        _state.machine.ProcessPhysics(delta);
    }
}
