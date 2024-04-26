using Godot;
using System;

public partial class AttackComponent : Node
{
    [Export]
	public int Attack { get; private set; } = 0;
}
