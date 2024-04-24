using Godot;
using System;

public partial class UI_HpBar : ProgressBar
{
	HealthComponent _health;
	public override void _Ready()
	{
		_health =  GetParent().GetChildByType<HealthComponent>();
		_health.OnHpModifiedNotifier += OnHpModified;
	}

	
	public override void _Process(double delta)
	{
	}

	void OnHpModified()
	{
		Value = (_health.Hp / (float)_health.MaxHp);
	}
}
