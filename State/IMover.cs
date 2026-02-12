using Godot;

public partial interface IMover
{
    public abstract Vector2 GetInputDirection();
    public abstract bool WantsJump();
}
