using Godot;
using System;

namespace Game.Common;

public partial class PenCursor : Sprite2D
{
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Hidden;
	}

    public override void _PhysicsProcess(double delta)
    {
		GlobalPosition = new Vector2(
			(float)Mathf.Lerp(GlobalPosition.X, GetGlobalMousePosition().X, 16.5 * delta),
			(float)Mathf.Lerp(GlobalPosition.Y, GetGlobalMousePosition().Y, 16.5 * delta)
		);

		float desiredRotation = Input.IsActionPressed("click") ? -12.5f : 0.0f;
		RotationDegrees = (float)Mathf.Lerp(RotationDegrees, desiredRotation, 16.5 * delta);

		Vector2 desiredScale = Input.IsActionPressed("click") ? Vector2.One : new Vector2(0.95f, 0.95f);
		Scale = new Vector2(
			(float)Mathf.Lerp(Scale.X, desiredScale.X, 16.5 * delta),
			(float)Mathf.Lerp(Scale.Y, desiredScale.Y, 16.5 * delta)
		);
    }
}
