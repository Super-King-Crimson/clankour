using Godot;
using System;

public abstract partial class State : Node
{
    public abstract void Enter(State prevState);
    public abstract void Exit(State nextState);
}
