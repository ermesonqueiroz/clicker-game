using Game.Globals;
using Godot;
using System.Globalization;

namespace Game.Components;

[GlobalClass]
public partial class DamageLabelSpawnerComponent : Node
{
	[Export]
	private LabelSettings _labelSettings;

	public void SpawnLabel(float damage, Vector2 position)
	{
		var gameWorld = GameManager.Instance.GameWorld as Node2D;
		if (gameWorld == null)
		{
			throw new System.InvalidOperationException("GameWorld must be a Node2D to spawn damage labels.");
		}

		Vector2 spawnPosition = gameWorld.ToLocal(position);
		Vector2 labelSize = new(100.0f, 36.0f);
		Vector2 labelPivot = labelSize / 2.0f;
		Label label = new()
		{
			Text = $"-{damage.ToString("0.#", CultureInfo.InvariantCulture)}",
			LabelSettings = _labelSettings,
			ZIndex = 1,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Size = labelSize,
			PivotOffset = labelPivot,
			Position = spawnPosition + new Vector2(
				(float)GD.RandRange(-12.0f, 12.0f),
				(float)GD.RandRange(-8.0f, 8.0f)
			) - labelPivot,
			Scale = new Vector2(0.65f, 0.65f),
			Modulate = Colors.White
		};

		label.TreeEntered += () => AnimateLabel(label);
		GameManager.Instance.AddChild(label);
	}

	private void AnimateLabel(Label label)
	{
		Vector2 targetPosition = label.Position + new Vector2(
			(float)GD.RandRange(-10.0f, 10.0f),
			-42.0f
		);

		Tween scaleTween = GetTree().CreateTween();
		scaleTween.TweenProperty(label, "scale", new Vector2(1.2f, 1.2f), 0.12f)
			.SetTrans(Tween.TransitionType.Back)
			.SetEase(Tween.EaseType.Out);
		scaleTween.TweenProperty(label, "scale", Vector2.One, 0.1f)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out);

		Tween motionTween = GetTree().CreateTween();
		motionTween
			.TweenProperty(label, "position", targetPosition, 0.7f)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.Out);

		Tween fadeTween = GetTree().CreateTween();
		fadeTween.TweenProperty(label, "modulate:a", 0.0f, 0.25f)
			.SetDelay(0.45f);
		fadeTween.TweenCallback(Callable.From(label.QueueFree));
	}
}
