using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [Export] protected State _state = null!;

    public override void _PhysicsProcess(double delta)
    {
        _state.ProcessPhysics(delta);
    }

    public override void _UnhandledInput(InputEvent e)
    {
        _state.ProcessInput(e);
    }
}
