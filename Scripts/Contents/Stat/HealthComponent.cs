using Godot;
using System;

public partial class HealthComponent : Node
{
    enum HealthState
    {
        Invincible = -1,
        Basic= 0,
    }

    public Action OnHpZeroNotifier; 
    public Action OnMaxHpModifiedNotifier;
    public Action OnHpModifiedNotifier;

    int _hp;
    int _maxHp;

    [Export]
    public int Hp
    {
        get { return _hp; }
        private set
        {
            _hp = value;
            OnHpModifiedNotifier?.Invoke();
            if (value <= 0)
                OnHpZeroNotifier?.Invoke();
        }
    }
    [Export]
    public int MaxHp { 
        get { return _maxHp; }
        private set
        {
            _maxHp = value;
            OnMaxHpModifiedNotifier?.Invoke();
        }
    }
    [Export]
    HealthState HpState { get; set; } = HealthState.Basic;

    #region Godot
    public override void _Ready()
    {
        Hp = MaxHp;
    }

    public override void _Process(double delta)
    {
    }
    #endregion

    public void GetDamaged(int attack)
    {
        if (HpState == HealthState.Invincible)
            return;
        Hp = Math.Max( Hp-attack, 0);
    }
    
}
