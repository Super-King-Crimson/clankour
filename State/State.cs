using Godot;
using System;

public abstract partial class State : Node
{
    public readonly string id;

    public State(string id) => this.id = id;

    public abstract State Enter(State prevState);
    public abstract State Exit(State nextState);
    public abstract State ProcessFrame(double delta);
    public abstract State ProcessInput(InputEvent e);
    public abstract State ProcessPhysics(double delta);
}
