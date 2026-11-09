using Godot;
using System;
using System.Threading.Tasks;

namespace Game.Components;

[GlobalClass]
public partial class HitBoxComponent : Area2D
{
	[Export]
	public float Damage = 1.0f;

	[Export]
	private CollisionShape2D _collisionShape;

	private bool _isStriking;

	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
		Monitoring = false;
		Monitorable = false;
	}

	public void SetRadius(float radius)
	{
		if (!float.IsFinite(radius) || radius <= 0.0f)
		{
			throw new ArgumentOutOfRangeException(nameof(radius), "Hit radius must be finite and greater than zero.");
		}

		if (_collisionShape.Shape is not CircleShape2D circleShape)
		{
			throw new InvalidOperationException("HitBox requires a CircleShape2D collision shape.");
		}

		circleShape.Radius = radius;
	}

	public async Task Strike(Vector2 position)
	{
		if (!float.IsFinite(Damage) || Damage <= 0.0f)
		{
			throw new InvalidOperationException("HitBox damage must be finite and greater than zero.");
		}

		if (_isStriking)
		{
			return;
		}

		var collisionShapeWasDisabled = _collisionShape.Disabled;
		_isStriking = true;

		try
		{
			Monitoring = false;
			GlobalPosition = position;
			_collisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
			await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
			Monitoring = true;
			await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
		}
		finally
		{
			Monitoring = false;
			if (collisionShapeWasDisabled)
			{
				_collisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
			}
			_isStriking = false;
		}
	}

	public async void StrikeFromAnimation()
	{
		try
		{
			await Strike(GlobalPosition);
		}
		catch (InvalidOperationException exception)
		{
			GD.PushError(exception.Message);
		}
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area is HurtBox hurtBox)
		{
			hurtBox.ReceiveHit(Damage);
		}
	}
}
