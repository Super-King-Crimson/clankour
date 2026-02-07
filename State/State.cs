using Godot;
using System;

public abstract partial class State : Node
{
    public abstract void Enter();
    public abstract void Exit();
}
