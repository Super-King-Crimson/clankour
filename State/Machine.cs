using Godot;
using System;
using System.Collections.Generic;
using Id = SMNodeId;

public interface Machine
{
    public void Init();

    public void ChangeState(Id id);
    public bool TryChangeState(Id id);

    public void AddState(SMNode node);
    public bool TryAddState(SMNode node);
    public void AddChildStatesOfNode(Node parentNode, bool recursive = false);

    public bool TryRemoveState(Id id);

    public Dictionary<SMNodeId, SMNode> GetStates();
}
