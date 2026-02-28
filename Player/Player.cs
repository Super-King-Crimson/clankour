using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [Export] protected State _state = null!;
}
