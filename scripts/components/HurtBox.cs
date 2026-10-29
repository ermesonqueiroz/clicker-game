using Godot;
using System;

namespace Game.Components;

[GlobalClass]
public partial class HurtBox : Area2D
{
	private bool _enabled = true;

	[Signal]
	public delegate void HitReceivedEventHandler(float damage);

	public bool Enabled
	{
		get => _enabled;
		set
		{
			_enabled = value;
			SetDeferred("monitorable", value);
		}
	}

	public void ReceiveHit(float damage)
	{
		if (!float.IsFinite(damage) || damage <= 0.0f)
		{
			throw new ArgumentOutOfRangeException(nameof(damage), "Damage must be finite and greater than zero.");
		}

		if (Enabled)
		{
			EmitSignal(SignalName.HitReceived, damage);
		}
	}
}
