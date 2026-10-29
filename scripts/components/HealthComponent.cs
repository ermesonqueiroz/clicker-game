using Godot;
using System;

namespace Game.Components;

[GlobalClass]
public partial class HealthComponent : Node
{
	[Export]
	public float MaxHealth = 10.0f;

	public float CurrentHealth { get; private set; }

	[Signal]
	public delegate void HealthChangedEventHandler(float currentHealth, float maxHealth);

	[Signal]
	public delegate void DiedEventHandler();

	public override void _Ready()
	{
		if (!float.IsFinite(MaxHealth) || MaxHealth <= 0.0f)
		{
			throw new InvalidOperationException("MaxHealth must be greater than zero.");
		}

		CurrentHealth = MaxHealth;
	}

	public void TakeDamage(float amount)
	{
		if (!float.IsFinite(amount) || amount <= 0.0f)
		{
			throw new ArgumentOutOfRangeException(nameof(amount), "Damage must be finite and greater than zero.");
		}

		if (CurrentHealth <= 0.0f)
		{
			return;
		}

		CurrentHealth = Mathf.Max(CurrentHealth - amount, 0.0f);
		EmitSignal(SignalName.HealthChanged, CurrentHealth, MaxHealth);

		if (CurrentHealth == 0.0f)
		{
			EmitSignal(SignalName.Died);
		}
	}

	public void ResetHealth()
	{
		CurrentHealth = MaxHealth;
		EmitSignal(SignalName.HealthChanged, CurrentHealth, MaxHealth);
	}
}
