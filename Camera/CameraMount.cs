using Godot;
using System;

public partial class CameraMount : Node3D
{
    [Export] public float camVerticalConstraintDeg = 60f;
    [Export] public float camSensH = 0.5f;
    [Export] public float camSensV = 0.5f;
    [Export] public bool camInvertH = true;
    [Export] public bool camInvertV = true;

    [Export] public CharacterBody3D Agent { get; set; } = null!;

    private bool _capturingMouse = false;
    [Export]
    public bool CapturingMouse
    {
        get => _capturingMouse;
        set
        {
            if (_capturingMouse != value)
            {
                _capturingMouse = value;
                ToggleMouseCapture();
            }
        }
    }

    public void ToggleMouseCapture()
    {
        if (_capturingMouse)
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }
        else
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
    }

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseMotion motionEvent && CapturingMouse)
        {
            int invH = camInvertH ? -1 : 1;
            int invV = camInvertV ? -1 : 1;

            Agent.RotateY(invH * camSensH * Mathf.DegToRad(motionEvent.Relative.X));
            this.RotateX(invV * camSensV * Mathf.DegToRad(motionEvent.Relative.Y));

            this.RotationDegrees = new Vector3(
                Mathf.Clamp(this.RotationDegrees.X, -1 * camVerticalConstraintDeg, camVerticalConstraintDeg),
                this.RotationDegrees.Y,
                this.RotationDegrees.Z
            );
        }
        else if (e.IsActionPressed("camlock") && !e.IsEcho())
        {
            CapturingMouse = !CapturingMouse;
        }
    }
}
