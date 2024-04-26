using Godot;
using System;
[Tool]
public partial class ContinuosArea2D : Area2D
{
	public override void _Ready()
	{
	}

    public override void _PhysicsProcess(double delta)
    {
        Monitoring = !Monitoring;
    }

}
