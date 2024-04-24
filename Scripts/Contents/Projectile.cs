using Godot;
using System;

public partial class Projectile : Sprite2D
{
    [Export]
    float _speed = 400f;

    Area2D _area;
    VisibleOnScreenNotifier2D _visibleOnScreenNotifier;

    public override void _Ready()
    {
        _area = GetNode<Area2D>("Area2D");
        _area.BodyEntered += OnBodyEnter;
        _visibleOnScreenNotifier =  this.GetChildByType<VisibleOnScreenNotifier2D>();
        _visibleOnScreenNotifier.ScreenExited += OnScreenExited;
    }

    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition += new Vector2(_speed * (float)delta, 0);
    }

    //area 2D
    void OnBodyEnter(Node2D body)
    {
        GD.Print($"Projectile Hit to {body.Name}");
        var attack = this.GetChildByType<AttackComponent>();
        body.TryGetChildByType<HealthComponent>()?.GetDamaged(attack.Attack);
    }

    void OnScreenExited()
    {
        Managers.Resource.Destroy(this);
    }


}
