#nullable enable

using Godot;
using System;

public partial class CameraMount : Node3D
{
    [Export] public float camVerticalConstraintDeg = 60f;
    [Export] public float camSensH = 0.5f;
    [Export] public float camSensV = 0.5f;
    [Export] public bool camInvertH = true;
    [Export] public bool camInvertV = true;

    [Export] public Node3D? character;

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseMotion motionEvent)
        {
            int invH = camInvertH ? -1 : 1;
            int invV = camInvertV ? -1 : 1;

            character?.RotateY(invH * camSensH * Mathf.DegToRad(motionEvent.Relative.X));
            this.RotateX(invV * camSensV * Mathf.DegToRad(motionEvent.Relative.Y));

            this.RotationDegrees = new Vector3(
                Mathf.Clamp(this.RotationDegrees.X, -1 * camVerticalConstraintDeg, camVerticalConstraintDeg),
                this.RotationDegrees.Y,
                this.RotationDegrees.Z
            );
        }
    }

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }
}
