using Godot;
using System;

public partial class Mob : CharacterBody2D
{
    [Export]
    float _speed = 200f;

    public override void _Ready()
    {
        HealthComponent health = this.GetChildByType<HealthComponent>();
        health.OnHpZeroNotifier += OnHpZero;

    }

    public override void _Process(double delta)
    {
        HealthComponent health = this.GetChildByType<HealthComponent>();
    }


    public override void _PhysicsProcess(double delta)
    {
        Velocity = new Vector2(-_speed, 0);
        MoveAndSlide();
    }

    void OnHpZero()
    {
        GD.Print($"{Name} :  Dead!");
        QueueFree();
    }


}
