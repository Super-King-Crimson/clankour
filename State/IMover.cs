using Godot;

public partial interface IMover
{
    public abstract Vector3 GetInputDirection();
    public abstract bool WantsJump();
}
