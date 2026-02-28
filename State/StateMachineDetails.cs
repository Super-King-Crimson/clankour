using Godot;
using System;

public partial class StateMachineDetails : Node
{
    public CharacterBody3D Agent { get; set; } = null!;
    public AnimationPlayer Animator { get; set; } = null!;
    public Mover Mover { get; set; } = null!;
    public Node3D Character { get; set; } = null!;
    public bool IsInitialized { get; protected set; } = false;

    public float RunSpeed { get; set; } = 5;

    public void Init(CharacterBody3D agent, AnimationPlayer animator, Mover mover, Node3D character)
    {
        if (IsInitialized) GD.PrintErr("StateMachineDetails initialized more than once, is this error?");

        IsInitialized = true;

        GD.Print(agent.Name, animator.Name, mover.Name, character.Name);

        Agent = agent;
        Animator = animator;
        Mover = mover;
        Character = character;
    }

    public Vector3 GetVelocityH() => new Vector3(Agent.Velocity.X, 0, Agent.Velocity.Z);
    public float GetSpeedH() => GetVelocityH().Length();

    public Vector3 GetVelocityV() => new Vector3(0, Agent.Velocity.Y, 0);
    public float GetSpeedV() => Agent.Velocity.Y;

    public Vector3 GetPosition() => Agent.Position;
    public Transform3D GetTransform() => Agent.Transform;
}
