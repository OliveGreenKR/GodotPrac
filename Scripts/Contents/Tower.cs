using Godot;
using System;

public partial class Tower : Sprite2D
{
    //PackedScene _projectile;
   

    public override void _Ready()
    {
        var timer = GetNode<Timer>("./Timer");

        //_projectile = GD.Load<PackedScene>("res://Scenes/projectile.tscn");
        timer.Timeout += () =>
        {
            //var pro = _projectile.Instantiate();
            //AddChild(pro);
            Managers.Resource.Instantiate("res://Scenes/projectile.tscn", this);
        };
        timer.Autostart = true;
        timer.Start(0.5f); ;
    }

    public override void _PhysicsProcess(double delta)
    {
       
    }

}
