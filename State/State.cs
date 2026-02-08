using Godot;
using System;

public abstract partial class State : Node
{
    public abstract State Enter(State prevState);
    public abstract State Exit(State nextState);
}
