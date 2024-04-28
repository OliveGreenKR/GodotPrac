using Godot;
using System;

public partial class Tower : Sprite2D
{
    //PackedScene _projectile;
   

    public override void _Ready()
    {
        

        var timer = GetNode<Timer>("./Timer");
        var projectile = Managers.Resource.LoadPackedScene<Projectile>(Define.Scenes.Nodes);
        timer.Timeout += () =>
        {
            Managers.Resource.Instantiate(projectile, this);
        };
        timer.Autostart = true;
        timer.Start(0.5f); ;
    }

    public override void _PhysicsProcess(double delta)
    {
       
    }

}
