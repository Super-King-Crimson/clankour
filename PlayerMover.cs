using Godot;
using System;

public partial class PlayerMover : Node, IMover
{
    private CameraMount _cameraMount;

    public void Init(CameraMount cameraMount) => _cameraMount = cameraMount;

    public Vector2 GetInputDirection() => Input.GetVector("left", "right", "fore", "back");
    public bool WantsJump() => Input.IsActionJustPressed("jump");
}
