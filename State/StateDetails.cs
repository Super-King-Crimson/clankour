using Godot;
using System;

public abstract partial class BaseStateDetails<AgentType> : Node
where AgentType : Node
{
    public abstract AgentType Agent { get; protected set; }
}

public partial class StateDetails : BaseStateDetails<CharacterBody3D>
{
    public override CharacterBody3D Agent { get; protected set; } = null!;
    public AnimationPlayer Animator { get; protected set; } = null!;
    public Mover Mover { get; protected set; } = null!;

    public Node3D? Character { get; set; }

    public float RunSpeed { get; set; } = 5;

    public Vector3 GetVelocityH() => new Vector3(Agent.Velocity.X, 0, Agent.Velocity.Z);
    public float GetSpeedH() => GetVelocityH().Length();

    public Vector3 GetVelocityV() => new Vector3(0, Agent.Velocity.Y, 0);
    public float GetSpeedV() => Agent.Velocity.Y;

    public Vector3 GetPosition() => Agent.Position;
    public Transform3D GetTransform() => Agent.Transform;
}
