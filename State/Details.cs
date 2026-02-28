using Godot;
using System;
using static SMNode;
using Godot.Collections;

public partial class Details : Node
{
    [Export] public CharacterBody3D Agent { get; set; } = null!;
    [Export] public AnimationPlayer Animator { get; set; } = null!;
    [Export] public Mover Mover { get; set; } = null!;
    [Export] public Node3D Character { get; set; } = null!;
    [Export] public float RunSpeed { get; set; } = 5;

    public Vector3 GetVelocityH() => new Vector3(Agent.Velocity.X, 0, Agent.Velocity.Z);
    public float GetSpeedH() => GetVelocityH().Length();

    public Vector3 GetVelocityV() => new Vector3(0, Agent.Velocity.Y, 0);
    public float GetSpeedV() => Agent.Velocity.Y;

    public Vector3 GetPosition() => Agent.Position;
    public Transform3D GetTransform() => Agent.Transform;
}
